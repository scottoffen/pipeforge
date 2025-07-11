using System.Reflection;
using PipeForge.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace PipeForge;

public static class Pipeline
{
    internal static readonly string NoStepsFoundMessage = "No pipeline steps found for {0} in the specified assemblies.";
    internal static readonly string NumberStepsFoundMessage = "Discovered {0} pipeline steps for {1}.";
    internal static readonly string StepDiscoveredMessage = "Discovered pipeline step {0} [Order={1}, Env={2}]";

    internal static readonly string RunnerAlreadyRegisteredMessage = "Pipeline runner for {0} already registered. Skipping step registration.";
    internal static readonly string StepRegistrationMessage = "Registering pipeline step {0} with {1} lifetime";
    internal static readonly string RunnerRegistrationMessage = "Registering pipeline runner {0} with {1} lifetime";

    /// <summary>
    /// Creates a new instance of <see cref="PipelineBuilder{T}"/> for the specified context type.
    /// </summary>
    /// <remarks>
    /// This method is used to start building a pipeline for a specific type.
    /// It allows for fluent configuration of pipeline steps.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static PipelineBuilder<T> CreateFor<T>() where T : class
    {
        return new PipelineBuilder<T>();
    }

    /// <summary>
    /// Creates a new instance of <see cref="PipelineBuilder{T}"/> for the specified context type.
    /// </summary>
    /// <remarks>
    /// This method is used to start building a pipeline for a specific type.
    /// It allows for fluent configuration of pipeline steps.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static PipelineBuilder<T> CreateFor<T>(ILoggerFactory? loggerFactory) where T : class
    {
        return new PipelineBuilder<T>(loggerFactory);
    }

    /// <summary>
    /// Discovers pipeline steps for a specific context type from all assemblies in the current AppDomain.
    /// </summary>
    /// <remarks>
    /// Specifying an environment name allows for conditional discovery of pipeline steps based on the environment.
    /// Steps without an environment name will always be discovered.
    /// Steps with an environment name will only be discovered if the current environment matches the specified name.
    /// </remarks>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="logger"></param>
    /// <returns></returns>
    [ExcludeFromCodeCoverage]
    internal static IEnumerable<PipelineStepDescriptor> Discover<TContext>(
        ILogger? logger = null)
        where TContext : class
    {
        return Discover<TContext>(AppDomain.CurrentDomain.GetAssemblies(), null, logger);
    }

    /// <summary>
    /// Discovers pipeline steps for a specific context type from all assemblies in the current AppDomain.
    /// </summary>
    /// <remarks>
    /// Specifying an environment name allows for conditional discovery of pipeline steps based on the environment.
    /// Steps without an environment name will always be discovered.
    /// Steps with an environment name will only be discovered if the current environment matches the specified name.
    /// </remarks>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="environmentName"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    [ExcludeFromCodeCoverage]
    internal static IEnumerable<PipelineStepDescriptor> Discover<TContext>(
        string? environmentName,
        ILogger? logger = null)
        where TContext : class
    {
        return Discover<TContext>(AppDomain.CurrentDomain.GetAssemblies(), environmentName, logger);
    }

    /// <summary>
    /// Discovers pipeline steps for a specific context type from the assembly containing the provided type marker.
    /// </summary>
    /// <remarks>
    /// Specifying an environment name allows for conditional discovery of pipeline steps based on the environment.
    /// Steps without an environment name will always be discovered.
    /// Steps with an environment name will only be discovered if the current environment matches the specified name.
    /// </remarks>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="typeMarker"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    [ExcludeFromCodeCoverage]
    internal static IEnumerable<PipelineStepDescriptor> Discover<TContext>(
        Type typeMarker,
        ILogger? logger = null)
        where TContext : class
    {
        return Discover<TContext>(new[] { typeMarker.Assembly }, null, logger);
    }

    /// <summary>
    /// Discovers pipeline steps for a specific context type from assembly containing the provided type marker.
    /// </summary>
    /// <remarks>
    /// Specifying an environment name allows for conditional discovery of pipeline steps based on the environment.
    /// Steps without an environment name will always be discovered.
    /// Steps with an environment name will only be discovered if the current environment matches the specified name.
    /// </remarks>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="typeMarker"></param>
    /// <param name="environmentName"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    [ExcludeFromCodeCoverage]
    internal static IEnumerable<PipelineStepDescriptor> Discover<TContext>(
        Type typeMarker,
        string? environmentName,
        ILogger? logger = null)
        where TContext : class
    {
        return Discover<TContext>(new[] { typeMarker.Assembly }, environmentName, logger);
    }

    /// <summary>
    /// Discovers pipeline steps for a specific context type from the provided assembly.
    /// </summary>
    /// <remarks>
    /// Specifying an environment name allows for conditional discovery of pipeline steps based on the environment.
    /// Steps without an environment name will always be discovered.
    /// Steps with an environment name will only be discovered if the current environment matches the specified name.
    /// </remarks>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="assembly"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    [ExcludeFromCodeCoverage]
    internal static IEnumerable<PipelineStepDescriptor> Discover<TContext>(
        Assembly assembly,
        ILogger? logger = null)
        where TContext : class
    {
        return Discover<TContext>(new[] { assembly }, null, logger);
    }

    /// <summary>
    /// Discovers pipeline steps for a specific context type from the provided assembly.
    /// </summary>
    /// <remarks>
    /// Specifying an environment name allows for conditional discovery of pipeline steps based on the environment.
    /// Steps without an environment name will always be discovered.
    /// Steps with an environment name will only be discovered if the current environment matches the specified name.
    /// </remarks>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="assembly"></param>
    /// <param name="environmentName"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    [ExcludeFromCodeCoverage]
    internal static IEnumerable<PipelineStepDescriptor> Discover<TContext>(
        Assembly assembly,
        string? environmentName,
        ILogger? logger = null)
        where TContext : class
    {
        return Discover<TContext>(new[] { assembly }, environmentName, logger);
    }

    /// <summary>
    /// Discovers pipeline steps for a specific context type from the provided assemblies.
    /// </summary>
    /// <remarks>
    /// Specifying an environment name allows for conditional discovery of pipeline steps based on the environment.
    /// Steps without an environment name will always be discovered.
    /// Steps with an environment name will only be discovered if the current environment matches the specified name.
    /// </remarks>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="assemblies"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    [ExcludeFromCodeCoverage]
    internal static IEnumerable<PipelineStepDescriptor> Discover<TContext>(
        IEnumerable<Assembly> assemblies,
        ILogger? logger = null)
        where TContext : class
    {
        return Discover<TContext>(assemblies, null, logger);
    }

    /// <summary>
    /// Discovers pipeline steps for a specific context type from the provided assemblies.
    /// </summary>
    /// <remarks>
    /// Specifying an environment name allows for conditional discovery of pipeline steps based on the environment.
    /// Steps without an environment name will always be discovered.
    /// Steps with an environment name will only be discovered if the current environment matches the specified name.
    /// </remarks>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="assemblies"></param>
    /// <param name="environmentName"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    internal static IEnumerable<PipelineStepDescriptor> Discover<TContext>(
        IEnumerable<Assembly> assemblies,
        string? environmentName,
        ILogger? logger = null)
        where TContext : class
    {
        var contextType = typeof(TContext);
        var stepInterface = typeof(IPipelineStep<TContext>);

        var descriptors = assemblies
            .SelectMany(a => SafeGetTypes(() => a.GetTypes()))
            .Where(t => !t.IsAbstract && !t.IsInterface && stepInterface.IsAssignableFrom(t) && t.GetCustomAttribute<PipelineStepAttribute>() != null)
            .Select(t => new PipelineStepDescriptor(t))
            .Where(d => d.Environment == null || d.Environment.Equals(environmentName, StringComparison.OrdinalIgnoreCase))
            .OrderBy(d => d.Order)
            .ToList();

        if (logger is not null)
        {
            if (descriptors.Any())
            {
                logger.LogDebug(NumberStepsFoundMessage, descriptors.Count, contextType.FullName ?? contextType.Name);

                foreach (var descriptor in descriptors)
                {
                    logger.LogDebug(
                        StepDiscoveredMessage,
                        descriptor.ImplementationType.FullName ?? descriptor.ImplementationType.Name,
                        descriptor.Order,
                        descriptor.Environment ?? "(none)");
                }
            }
            else
            {
                logger.LogDebug(NoStepsFoundMessage, contextType.FullName ?? contextType.Name);
            }
        }

        return descriptors;
    }

    /// <summary>
    /// Registers pipeline steps for a specific context type using the provided descriptors.
    /// </summary>
    /// <remarks>
    /// This method will register all pipeline steps defined in the provided descriptors
    /// as transient services in the service collection.
    /// </remarks>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="descriptors"></param>
    /// <param name="logger"></param>
    [ExcludeFromCodeCoverage]
    internal static void Register<TContext>(
        IServiceCollection services,
        IEnumerable<PipelineStepDescriptor> descriptors,
        ILogger? logger = null)
        where TContext : class
    {
        Register<TContext>(services, descriptors, ServiceLifetime.Transient, logger);
    }

    /// <summary>
    /// Registers pipeline steps for a specific context type.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="descriptors"></param>
    /// <param name="lifetime"></param>
    /// <param name="logger"></param>
    internal static void Register<TContext>(
        IServiceCollection services,
        IEnumerable<PipelineStepDescriptor> descriptors,
        ServiceLifetime lifetime,
        ILogger? logger = null)
        where TContext : class
    {
        var runnerType = typeof(IPipelineRunner<TContext>);
        if (services.Any(s => s.ServiceType == runnerType))
        {
            logger?.LogDebug(RunnerAlreadyRegisteredMessage, typeof(TContext).FullName ?? typeof(TContext).Name);
            return;
        }

        var interfaceType = typeof(IPipelineStep<TContext>);
        var lazyType = typeof(Lazy<>).MakeGenericType(interfaceType);
        foreach (var descriptor in descriptors)
        {
            RegisterStep(descriptor.ImplementationType, interfaceType, lazyType, services, lifetime, logger);
        }

        logger?.LogDebug(RunnerRegistrationMessage, runnerType.FullName ?? runnerType.Name, lifetime);

        services.TryAdd(ServiceDescriptor.Describe(runnerType, typeof(PipelineRunner<TContext>), lifetime));
    }

    /// <summary>
    /// Registers a single pipeline step implementation type with the service collection.
    /// This method is used internally to register pipeline steps discovered from assemblies,
    /// as well as explicitly registered steps from <see cref="IServiceCollection"/> extensions.
    /// It registers the step as both an implementation of <c>IPipelineStep&lt;T&gt;</c> and as a concrete type.
    /// It also registers a <c>Lazy&lt;IPipelineStep&lt;T&gt;&gt;</c> for deferred resolution.
    /// </summary>
    /// <param name="concreteType"></param>
    /// <param name="interfaceType"></param>
    /// <param name="lazyType"></param>
    /// <param name="services"></param>
    /// <param name="lifetime"></param>
    /// <param name="logger"></param>
    /// <exception cref="InvalidOperationException"></exception>
    internal static void RegisterStep(
        Type concreteType,
        Type interfaceType,
        Type lazyType,
        IServiceCollection services,
        ServiceLifetime lifetime,
        ILogger? logger)
    {
        logger?.LogDebug(StepRegistrationMessage, concreteType.FullName ?? concreteType.Name, lifetime);

        // Register the IPipelineStep<T> implementation
        services.TryAddEnumerable(ServiceDescriptor.Describe(interfaceType, concreteType, lifetime));

        // Register the concrete implementation type
        services.Add(ServiceDescriptor.Describe(concreteType, concreteType, lifetime));

        // Register Lazy<IPipelineStep<T>>
        services.Add(ServiceDescriptor.Describe(lazyType, sp =>
        {
            var funcType = typeof(Func<>).MakeGenericType(interfaceType);

            var methodInfo = typeof(ServiceProviderServiceExtensions)
                .GetMethod(nameof(ServiceProviderServiceExtensions.GetRequiredService), new[] { typeof(IServiceProvider) });

            if (methodInfo is null)
                throw new InvalidOperationException("Could not find GetRequiredService<T>(IServiceProvider).");

            var genericMethod = methodInfo.MakeGenericMethod(concreteType);

            var factoryDelegate = Delegate.CreateDelegate(funcType, sp, genericMethod);

            return Activator.CreateInstance(lazyType, factoryDelegate)
                ?? throw new InvalidOperationException($"Could not create Lazy<{interfaceType}>.");
        }, lifetime));
    }

    /// <summary>
    /// Safely retrieves all loadable types from the specified assembly, skipping those that cannot be loaded.
    /// </summary>
    internal static IEnumerable<Type> SafeGetTypes(Func<Type[]> getTypes)
    {
        try
        {
            return getTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t is not null)!;
        }
        catch
        {
            return Enumerable.Empty<Type>();
        }
    }
}
