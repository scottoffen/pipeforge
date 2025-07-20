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

        // Ensure that the context type does not implement IPipelineStep<TContext>
        if (contextType.GetInterfaces().Contains(typeof(IPipelineStep<>)))
        {
            logger?.LogError(MessageInvalidContextType, contextTypeName);
            throw new InvalidOperationException(string.Format(MessageInvalidContextType, contextTypeName));
        }

        // Register the pipeline runner first to ensure it is available for step registration.
        if (!services.RegisterRunner<TContext, TStepInterface, TRunnerInterface>(assemblies, lifetime, logger))
        {
            return services;
        }

        // Get each descriptor for the pipeline.
        var descriptors = assemblies.FindImplementationsOf<TStepInterface>()
        .Select(t => new PipelineStepDescriptor(t))
        .Where(descriptor =>
            !descriptor.Filters.Any() ||
            (filters is not null && descriptor.Filters.Any(f =>
                filters.Any(filter => string.Equals(filter, f, StringComparison.OrdinalIgnoreCase))))
        )
        .OrderBy(descriptor => descriptor.Order);

        // Register each step in the pipeline
        var counter = 0;
        foreach (var descriptor in descriptors)
        {
            if (descriptor is null) continue;
            counter++;

            var typeDescriptor = TypeDescriptor.Create<TStepInterface>(descriptor.ImplementationType);
            logger?.LogDebug(MessageStepDiscovered, typeDescriptor.TypeName, descriptor.Order, string.Join(",", descriptor.Filters));

            services.RegisterStep(typeDescriptor, lifetime, logger);
        }

        // Log the number of steps registered
        if (counter == 0)
        {
            logger?.LogWarning(MessageNoStepsFound, contextTypeName);
        }
        else
        {
            logger?.LogInformation(MessageNumberStepsFound, counter, contextTypeName);
        }

        return services;
    }

    private static bool RegisterRunner<TContext, TStepInterface, TRunnerInterface>(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime lifetime,
        ILogger? logger)
        where TContext : class
        where TStepInterface : IPipelineStep<TContext>
        where TRunnerInterface : IPipelineRunner<TContext, TStepInterface>
    {
        // Stop pipeline registration early if the runner is already registered.
        var runnerType = typeof(TRunnerInterface);
        var runnerTypeName = runnerType.FullName ?? runnerType.Name;
        if (services.Any(service => service.ServiceType == runnerType))
        {
            logger?.LogWarning(MessageRunnerAlreadyRegistered, runnerTypeName);
            return false;
        }

        // If the runner is assignable from the default runner type, register it directly.
        var defaultRunnerType = typeof(PipelineRunner<TContext, TStepInterface>);
        if (defaultRunnerType == runnerType)
        {
            var defaultRunnerTypeName = defaultRunnerType.FullName ?? defaultRunnerType.Name;
            // Direct match — safe to register default implementation
            logger?.LogDebug(MessageRunnerRegistration, defaultRunnerTypeName, runnerTypeName, lifetime.ToString());
            services.TryAdd(ServiceDescriptor.Describe(runnerType, defaultRunnerType, lifetime));
            return true;
        }

        // Search for a concrete implementation of TRunnerInterface in the provided assemblies
        var concreteRunner = assemblies
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t =>
                !t.IsAbstract &&
                !t.IsInterface &&
                runnerType.IsAssignableFrom(t));

        // If a concrete implementation is found, register it
        if (concreteRunner is not null)
        {
            var concreteRunnerName = concreteRunner.FullName ?? concreteRunner.Name;
            logger?.LogDebug(MessageRunnerRegistration, concreteRunnerName, runnerTypeName, lifetime.ToString());
            services.TryAdd(ServiceDescriptor.Describe(runnerType, concreteRunner, lifetime));
            return true;
        }

        // If no concrete implementation is found, log a warning and throw an exception
        logger?.LogWarning(MessageRunnerImplementationNotFound, runnerTypeName);
        throw new InvalidOperationException(string.Format(MessageRunnerImplementationNotFound, runnerTypeName));
    }

    internal static void RegisterStep(
        this IServiceCollection services,
        TypeDescriptor descriptor,
        ServiceLifetime lifetime,
        ILogger? logger)
    {
        if (services.Any(s => s.ServiceType == descriptor.InterfaceType && s.ImplementationType == descriptor.ConcreteType))
        {
            logger?.LogWarning(MessageStepAlreadyRegistered, descriptor.TypeName);
#if DEBUG
            throw new InvalidOperationException(string.Format(MessageStepAlreadyRegistered, descriptor.TypeName));
#else
            return;
#endif
        }

        logger?.LogDebug(MessageStepRegistration, descriptor.TypeName, lifetime);

        // Register the IPipelineStep<T> implementation
        services.TryAddEnumerable(ServiceDescriptor.Describe(descriptor.InterfaceType, descriptor.ConcreteType, lifetime));

        // Register the concrete implementation type
        services.TryAdd(ServiceDescriptor.Describe(descriptor.ConcreteType, descriptor.ConcreteType, lifetime));

        // Register Lazy<IPipelineStep<T>>
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
