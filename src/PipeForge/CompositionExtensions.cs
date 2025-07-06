using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PipeForge;

public static class CompositionExtensions
{
    private static readonly ConcurrentDictionary<Type, Type> _lazyStepTypes = new();

    private static readonly Type loggerFactoryType = typeof(ILoggerFactory);
    private static readonly Type _openGenericPipelineStepType = typeof(IPipelineStep<>);

    private static ILoggerFactory? _loggerFactory = null;

    internal static readonly string ArgumentExceptionMessage = "Type {0} does not implement the required interface IPipelineStep<T>.";

    internal static readonly string InvalidOperationExceptionMessage = "Pipeline step '{0}' is already registered. Pipeline steps must be uniquely registered.";

    /// <summary>
    /// Registers a pipeline step of type <typeparamref name="T"/> in the service collection. Pipeline steps must be uniquely registered in order to be discoverable by the <see cref="IPipelineRunner{T}"/>  .
    /// </summary>
    /// <remarks>
    /// By default, the step is registered as a transient service.
    /// If you want to register it with a different lifetime, you can specify that using the <paramref name="lifetime"/> parameter.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="services"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipelineStep<T>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where T : IPipelineStep
    {
        services.GetLoggerFactory();
        var logger = _loggerFactory?.CreateLogger(nameof(Pipeline));

        if (IsValidPipelineStep<T>(out var concreteType, out var interfaceType, out var lazyType))
        {
            if (!services.Any(s => s.ServiceType == concreteType))
            {
                Pipeline.RegisterStep(concreteType, interfaceType!, lazyType!, services, lifetime, logger);
            }
            else
            {
                logger?.LogWarning("Attempt to register pipeline step '{Step}' multiple times.", concreteType.FullName ?? concreteType.Name);
#if DEBUG
                throw new InvalidOperationException(string.Format(InvalidOperationExceptionMessage, concreteType.FullName ?? concreteType.Name));
#endif
            }
        }
        else
        {
            logger?.LogError("Type {Type} does not implement the required interface IPipelineStep<T>.", concreteType.FullName ?? concreteType.Name);
            throw new ArgumentException(string.Format(ArgumentExceptionMessage, concreteType.FullName ?? concreteType.Name));
        }

        return services;
    }

    /// <summary>
    /// Discovers and registers all pipeline steps in all assemblies in the AppDomain for the specified context type <typeparamref name="TContext"/> using ServiceLifetime.Transient.
    /// </summary>
    /// <remarks>
    /// Specifying an environment name allows for conditional registration of pipeline steps based on the environment.
    /// Steps without an environment name will always be registered.
    /// Steps with an environment name will only be registered if the current environment matches the specified name.
    /// </remarks>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipelineFor<TContext>(this IServiceCollection services)
        where TContext : class
    {
        return services.AddPipelineFor<TContext>(AppDomain.CurrentDomain.GetAssemblies(), ServiceLifetime.Transient, null);
    }

    /// <summary>
    /// Discovers and registers all pipeline steps in all assemblies in the AppDomain for the specified context type <typeparamref name="TContext"/> using ServiceLifetime.Transient.
    /// </summary>
    /// <remarks>
    /// Specifying an environment name allows for conditional registration of pipeline steps based on the environment.
    /// Steps without an environment name will always be registered.
    /// Steps with an environment name will only be registered if the current environment matches the specified name.
    /// </remarks>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="environmentName"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipelineFor<TContext>(
        this IServiceCollection services,
        string? environmentName)
        where TContext : class
    {
        return services.AddPipelineFor<TContext>(AppDomain.CurrentDomain.GetAssemblies(), ServiceLifetime.Transient, environmentName);
    }

    /// <summary>
    /// Discovers and registers all pipeline steps in all assemblies in the AppDomain for the specified context type <typeparamref name="TContext"/> using the specified service lifetime.
    /// </summary>
    /// <remarks>
    /// Specifying an environment name allows for conditional registration of pipeline steps based on the environment.
    /// Steps without an environment name will always be registered.
    /// Steps with an environment name will only be registered if the current environment matches the specified name.
    /// </remarks>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="environmentName"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipelineFor<TContext>(
        this IServiceCollection services,
        ServiceLifetime lifetime,
        string? environmentName)
        where TContext : class
    {
        return services.AddPipelineFor<TContext>(AppDomain.CurrentDomain.GetAssemblies(), lifetime, environmentName);
    }

    /// <summary>
    /// Discovers and registers all pipeline steps in the assembly containing the marker type for the specified context type <typeparamref name="TContext"/> using ServiceLifetime.Transient.
    /// </summary>
    /// <remarks>
    /// Specifying an environment name allows for conditional registration of pipeline steps based on the environment.
    /// Steps without an environment name will always be registered.
    /// Steps with an environment name will only be registered if the current environment matches the specified name.
    /// </remarks>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="markerType"></param>
    /// <param name="environmentName"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipelineFor<TContext>(
        this IServiceCollection services,
        Type markerType,
        string? environmentName)
        where TContext : class
    {
        return services.AddPipelineFor<TContext>(new[] { markerType.Assembly }, ServiceLifetime.Transient, environmentName);
    }

    /// <summary>
    /// Discovers and registers all pipeline steps in the assembly containing the marker type for the specified context type <typeparamref name="TContext"/> using the specified service lifetime.
    /// </summary>
    /// <remarks>
    /// Specifying an environment name allows for conditional registration of pipeline steps based on the environment.
    /// Steps without an environment name will always be registered.
    /// Steps with an environment name will only be registered if the current environment matches the specified name.
    /// </remarks>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="markerType"></param>
    /// <param name="environmentName"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipelineFor<TContext>(
        this IServiceCollection services,
        Type markerType,
        ServiceLifetime lifetime,
        string? environmentName)
        where TContext : class
    {
        return services.AddPipelineFor<TContext>(new[] { markerType.Assembly }, lifetime, environmentName);
    }

    /// <summary>
    /// Discovers and registers all pipeline steps in the specified assembly for the specified context type <typeparamref name="TContext"/> using ServiceLifetime.Transient.
    /// </summary>
    /// <remarks>
    /// Specifying an environment name allows for conditional registration of pipeline steps based on the environment.
    /// Steps without an environment name will always be registered.
    /// Steps with an environment name will only be registered if the current environment matches the specified name.
    /// </remarks>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="assembly"></param>
    /// <param name="environmentName"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipelineFor<TContext>(
        this IServiceCollection services,
        Assembly assembly,
        string? environmentName)
        where TContext : class
    {
        return services.AddPipelineFor<TContext>(new[] { assembly }, ServiceLifetime.Transient, environmentName);
    }

    /// <summary>
    /// Discovers and registers all pipeline steps in the specified assembly for the specified context type <typeparamref name="TContext"/> using the specified service lifetime.
    /// </summary>
    /// <remarks>
    /// Specifying an environment name allows for conditional registration of pipeline steps based on the environment.
    /// Steps without an environment name will always be registered.
    /// Steps with an environment name will only be registered if the current environment matches the specified name.
    /// </remarks>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="assembly"></param>
    /// <param name="environmentName"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipelineFor<TContext>(
        this IServiceCollection services,
        Assembly assembly,
        ServiceLifetime lifetime,
        string? environmentName)
        where TContext : class
    {
        return services.AddPipelineFor<TContext>(new[] { assembly }, lifetime, environmentName);
    }

    /// <summary>
    /// Discovers and registers all pipeline steps in the specified assemblies for the specified context type <typeparamref name="TContext"/> using ServiceLifetime.Transient.
    /// </summary>
    /// <remarks>
    /// Specifying an environment name allows for conditional registration of pipeline steps based on the environment.
    /// Steps without an environment name will always be registered.
    /// Steps with an environment name will only be registered if the current environment matches the specified name.
    /// </remarks>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    /// <param name="environmentName"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipelineFor<TContext>(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        string? environmentName)
        where TContext : class
    {
        return services.AddPipelineFor<TContext>(assemblies, ServiceLifetime.Transient, environmentName);
    }

    /// <summary>
    /// Discovers and registers all pipeline steps in the specified assemblies for the specified context type <typeparamref name="TContext"/> using the specified service lifetime.
    /// </summary>
    /// <remarks>
    /// Specifying an environment name allows for conditional registration of pipeline steps based on the environment.
    /// Steps without an environment name will always be registered.
    /// Steps with an environment name will only be registered if the current environment matches the specified name.
    /// </remarks>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    /// <param name="environmentName"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipelineFor<TContext>(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime lifetime,
        string? environmentName)
        where TContext : class
    {
        services.GetLoggerFactory();
        var logger = _loggerFactory?.CreateLogger(nameof(Pipeline));

        var descriptors = Pipeline.Discover<TContext>(assemblies, environmentName, logger);
        Pipeline.Register<TContext>(services, descriptors, lifetime, logger);

        return services;
    }

    /// <summary>
    /// Retrieves the <see cref="ILoggerFactory"/> from the service collection if it has been registered.
    /// If it has not been registered, it will build the service provider to retrieve it.
    /// </summary>
    /// <param name="services"></param>
    private static void GetLoggerFactory(this IServiceCollection services)
    {
        if (_loggerFactory == null && services.Any(s => s.ServiceType == loggerFactoryType))
        {
            var provider = services.BuildServiceProvider();
            _loggerFactory = provider.GetService<ILoggerFactory>();
        }
    }

    /// <summary>
    /// Returns true if the type implements the open generic <c>IPipelineStep&lt;T&gt;</c> interface.
    /// </summary>
    /// <param name="concreteType"></param>
    /// <param name="interfaceType"></param>
    /// <param name="lazyType"></param>
    /// <returns></returns>
    private static bool IsValidPipelineStep<T>(out Type concreteType, out Type? interfaceType, out Type? lazyType)
    {
        concreteType = typeof(T);

        interfaceType = concreteType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == _openGenericPipelineStepType);

        lazyType = interfaceType == null
            ? null
            : _lazyStepTypes.GetOrAdd(interfaceType, it => typeof(Lazy<>).MakeGenericType(it));

        return interfaceType != null;
    }
}
