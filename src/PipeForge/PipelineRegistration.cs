using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using PipeForge.Extensions;
using PipeForge.Metadata;

namespace PipeForge;

internal static class PipelineRegistration
{
    internal static readonly string MessageInvalidContextType = "The context type '{0}' cannot implement IPipelineStep<>.";
    internal static readonly string MessageNoStepsFound = "No pipeline steps found for {0} in the specified assemblies.";
    internal static readonly string MessageNumberStepsFound = "Discovered and registered {0} pipeline steps for {1}.";
    internal static readonly string MessageRunnerAlreadyRegistered = "Pipeline runner for {0} already registered. Skipping step registration.";
    internal static readonly string MessageRunnerImplementationNotFound = "No concrete implementation found for pipeline runner interface '{0}'. If you are using a custom runner interface, you must also provide an implementation for it.";
    internal static readonly string MessageRunnerRegistration = "Registering pipeline runner implementation {0} for interface {1} with {2} lifetime";
    internal static readonly string MessageStepAlreadyRegistered = "Pipeline step '{0}' is already registered. Pipeline steps must be uniquely registered.";
    internal static readonly string MessageStepDiscovered = "Discovered pipeline step {0} [Order={1}, Filter={2}]";
    internal static readonly string MessageStepRegistration = "Registering pipeline step {0} with {1} lifetime";

    public static IServiceCollection RegisterPipeline<TContext, TStepInterface, TRunnerInterface>(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime lifetime,
        string[]? filters)
        where TContext : class
        where TStepInterface : IPipelineStep<TContext>
        where TRunnerInterface : IPipelineRunner<TContext, TStepInterface>
    {
        var logger = services.GetLogger();
        var contextType = typeof(TContext);
        var contextTypeName = contextType.FullName ?? contextType.Name;

        // 0. Ensure that the context type does not implement IPipelineStep<>
        if (contextType.ImplementsPipelineStep())
        {
            logger?.LogError(MessageInvalidContextType, contextTypeName);
            throw new InvalidOperationException(string.Format(MessageInvalidContextType, contextTypeName));
        }

        // 1. Register the pipeline runner first. We don't want to register
        //    any steps if the runner cannot be resolved.
        if (!services.RegisterRunner<TContext, TStepInterface, TRunnerInterface>(assemblies, lifetime, logger))
        {
            return services;
        }

        // 2. Get descriptor for each step in the pipeline.
        var descriptors = assemblies
            .FindClosedImplementationsOf<TStepInterface>()
            .Select(t => new PipelineStepDescriptor(t))
            .Where(d => d.Filters.MatchesAnyFilter(filters))
            .OrderBy(d => d.Order);

        // 3. Register each discovered step in the pipeline.
        var counter = 0;
        foreach (var descriptor in descriptors)
        {
            if (descriptor is null) continue;

            counter++;
            var typeDescriptor = StepTypeDescriptor.Create<TStepInterface>(descriptor.ImplementationType);

            // 3a. Log the step discovery
            logger?.LogDebug(MessageStepDiscovered, typeDescriptor.TypeName, descriptor.Order, string.Join(",", descriptor.Filters));

            // 3b. Register the step
            services.RegisterStep(typeDescriptor, lifetime, logger);
        }

        // 4. Log the number of steps found and registered.
        logger?.LogStepsRegistered(counter, contextTypeName);

        return services;
    }

    public static bool RegisterRunner<TContext, TStepInterface, TRunnerInterface>(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime lifetime,
        ILogger? logger)
        where TContext : class
        where TStepInterface : IPipelineStep<TContext>
        where TRunnerInterface : IPipelineRunner<TContext, TStepInterface>
    {
        var runnerType = typeof(TRunnerInterface);
        var runnerTypeName = runnerType.FullName ?? runnerType.Name;

        // 0. Return early if the runner is already registered.
        if (services.Any(service => service.ServiceType == runnerType))
        {
            logger?.LogWarning(MessageRunnerAlreadyRegistered, runnerTypeName);
            return false;
        }

        // 1. Look for a concrete implementation of the runner interface.
        var concreteRunner = assemblies
            .FindClosedImplementationsOf<TRunnerInterface>()
            .FirstOrDefault(t => t.DirectlyImplements(runnerType));

        // 2. If a concrete implementation is found, register it.
        if (concreteRunner is not null)
        {
            var concreteRunnerName = concreteRunner.FullName ?? concreteRunner.Name;
            logger?.LogDebug(MessageRunnerRegistration, concreteRunnerName, runnerTypeName, lifetime.ToString());
            services.TryAdd(ServiceDescriptor.Describe(runnerType, concreteRunner, lifetime));
            return true;
        }

        // 3. Otherwise, attempt to register the default runner implementation.
        var defaultImplementation = typeof(DefaultPipelineRunner<TContext, TStepInterface>);
        if (runnerType.IsAssignableFrom(defaultImplementation))
        {
            logger?.LogDebug(MessageRunnerRegistration, defaultImplementation.FullName ?? defaultImplementation.Name, runnerTypeName, lifetime.ToString());
            services.TryAdd(ServiceDescriptor.Describe(runnerType, defaultImplementation, lifetime));
            return true;
        }

        // 4. No implementation found, and the default is not valid — throw
        logger?.LogWarning(MessageRunnerImplementationNotFound, runnerTypeName);
        throw new InvalidOperationException(string.Format(MessageRunnerImplementationNotFound, runnerTypeName));
    }

    public static void RegisterStep(
        this IServiceCollection services,
        StepTypeDescriptor descriptor,
        ServiceLifetime lifetime,
        ILogger? logger)
    {
        // 0. We absolutely do not want to register duplicate steps. Start by
        //    checking if the step is already registered. In debug mode, throw
        //    an exception if it is already registered.
        if (services.Any(s => s.ServiceType == descriptor.InterfaceType && s.ImplementationType == descriptor.ConcreteType))
        {
            logger?.LogWarning(MessageStepAlreadyRegistered, descriptor.TypeName);
#if DEBUG
            throw new InvalidOperationException(string.Format(MessageStepAlreadyRegistered, descriptor.TypeName));
#else
            return;
#endif
        }

        // 1. Log the initialization of step registration.
        logger?.LogDebug(MessageStepRegistration, descriptor.TypeName, lifetime);

        // 2. Register the IPipelineStep<T> implementation
        services.TryAddEnumerable(ServiceDescriptor.Describe(descriptor.InterfaceType, descriptor.ConcreteType, lifetime));

        // 3. Register the concrete implementation type
        services.TryAdd(ServiceDescriptor.Describe(descriptor.ConcreteType, descriptor.ConcreteType, lifetime));

        // 4. Register Lazy<IPipelineStep<T>>
        services.Add(ServiceDescriptor.Describe(descriptor.LazyType, sp =>
        {
            var funcType = typeof(Func<>).MakeGenericType(descriptor.InterfaceType);

            var methodInfo = typeof(ServiceProviderServiceExtensions)
                .GetMethod(nameof(ServiceProviderServiceExtensions.GetRequiredService), [typeof(IServiceProvider)])
                ?? throw new InvalidOperationException("Could not find GetRequiredService<T>(IServiceProvider).");

            var genericMethod = methodInfo.MakeGenericMethod(descriptor.ConcreteType);

            var factoryDelegate = Delegate.CreateDelegate(funcType, sp, genericMethod);

            return Activator.CreateInstance(descriptor.LazyType, factoryDelegate)
                ?? throw new InvalidOperationException($"Could not create Lazy<{descriptor.InterfaceType}>.");
        }, lifetime));
    }
}
