using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace PipeForge;

[ExcludeFromCodeCoverage]
public static class AddPipelineExtensions
{
    #region AddPipeline<TContext>

    /// <summary>
    /// Discovers and registers all pipeline steps in the <see cref="AppDomain.CurrentDomain"/> that implement <see cref="IPipelineStep{TContext}"/>.
    /// Steps are registered in the order defined by the <see cref="PipelineStepAttribute.Order"/> attribute,
    /// using the <see cref="ServiceLifetime.Transient"/> service lifetime. Also registers an instance of
    /// <see cref="PipelineRunner{TContext}"/> as <see cref="IPipelineRunner{TContext}"/>,
    /// enabling resolution of the pipeline runner from the dependency injection container.
    /// </summary>
    /// <remarks>
    ///     <para>Specifying a filter allows for conditional registration of pipeline steps based on the filter.</para>
    ///     <list type="bullet">
    ///         <item>Steps without a filter will always be registered.</item>
    ///         <item>Steps with a filter will only be registered if the current filter contains a match for any of the filter constraints on the step.</item>
    ///     </list>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when the type <c>TContext</c> implements the generic <see cref="IPipelineStep"/> or <see cref="IPipelineRunner{TContext}"/> interface.
    /// </exception>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipeline<TContext>(
        this IServiceCollection services,
        string[]? filters = null)
        where TContext : class
    {
        return services.AddPipeline<TContext, IPipelineStep<TContext>, IPipelineRunner<TContext>>(
            AppDomain.CurrentDomain.GetAssemblies(),
            ServiceLifetime.Transient,
            filters);
    }

    /// <summary>
    /// Discovers and registers all pipeline steps in the specified assemblies that implement <see cref="IPipelineStep{TContext}"/>.
    /// Steps are registered in the order defined by the <see cref="PipelineStepAttribute.Order"/> attribute,
    /// using the <see cref="ServiceLifetime.Transient"/> service lifetime. Also registers an instance of
    /// <see cref="PipelineRunner{TContext}"/> as <see cref="IPipelineRunner{TContext}"/>,
    /// enabling resolution of the pipeline runner from the dependency injection container.
    /// </summary>
    /// <remarks>
    ///     <para>Specifying a filter allows for conditional registration of pipeline steps based on the filter.</para>
    ///     <list type="bullet">
    ///         <item>Steps without a filter will always be registered.</item>
    ///         <item>Steps with a filter will only be registered if the current filter contains a match for any of the filter constraints on the step.</item>
    ///     </list>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when the type <c>TContext</c> implements the generic <see cref="IPipelineStep"/> or <see cref="IPipelineRunner{TContext}"/> interface.
    /// </exception>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipeline<TContext>(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        string[]? filters = null)
        where TContext : class
    {
        return services.AddPipeline<TContext, IPipelineStep<TContext>, IPipelineRunner<TContext>>(
            assemblies,
            ServiceLifetime.Transient,
            filters);
    }

    /// <summary>
    /// Discovers and registers all pipeline steps in the <see cref="AppDomain.CurrentDomain"/> that implement <see cref="IPipelineStep{TContext}"/>.
    /// Steps are registered in the order defined by the <see cref="PipelineStepAttribute.Order"/> attribute,
    /// using the specified <see cref="ServiceLifetime"/>. Also registers an instance of
    /// <see cref="PipelineRunner{TContext}"/> as <see cref="IPipelineRunner{TContext}"/>,
    /// enabling resolution of the pipeline runner from the dependency injection container.
    /// </summary>
    /// <remarks>
    ///     <para>Specifying a filter allows for conditional registration of pipeline steps based on the filter.</para>
    ///     <list type="bullet">
    ///         <item>Steps without a filter will always be registered.</item>
    ///         <item>Steps with a filter will only be registered if the current filter contains a match for any of the filter constraints on the step.</item>
    ///     </list>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when the type <c>TContext</c> implements the generic <see cref="IPipelineStep"/> or <see cref="IPipelineRunner{TContext}"/> interface.
    /// </exception>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="lifetime"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipeline<TContext>(
        this IServiceCollection services,
        ServiceLifetime lifetime,
        string[]? filters = null)
        where TContext : class
    {
        return services.AddPipeline<TContext, IPipelineStep<TContext>, IPipelineRunner<TContext>>(
            AppDomain.CurrentDomain.GetAssemblies(),
            lifetime,
            filters);
    }

    /// <summary>
    /// Discovers and registers all pipeline steps in the specified assemblies that implement <see cref="IPipelineStep{TContext}"/>.
    /// Steps are registered in the order defined by the <see cref="PipelineStepAttribute.Order"/> attribute,
    /// using the specified <see cref="ServiceLifetime"/>. Also registers an instance of
    /// <see cref="PipelineRunner{TContext}"/> as <see cref="IPipelineRunner{TContext}"/>,
    /// enabling resolution of the pipeline runner from the dependency injection container.
    /// </summary>
    /// <remarks>
    ///     <para>Specifying a filter allows for conditional registration of pipeline steps based on the filter.</para>
    ///     <list type="bullet">
    ///         <item>Steps without a filter will always be registered.</item>
    ///         <item>Steps with a filter will only be registered if the current filter contains a match for any of the filter constraints on the step.</item>
    ///     </list>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when the type <c>TContext</c> implements the generic <see cref="IPipelineStep"/> or <see cref="IPipelineRunner{TContext}"/> interface.
    /// </exception>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    /// <param name="lifetime"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipeline<TContext>(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime lifetime,
        string[]? filters = null)
        where TContext : class
    {
        return services.AddPipeline<TContext, IPipelineStep<TContext>, IPipelineRunner<TContext>>(
            assemblies,
            lifetime,
            filters);
    }

    #endregion

    #region AddPipeline<TContext, TStepInterface>

    /// <summary>
    /// Discovers and registers all pipeline steps in the <see cref="AppDomain.CurrentDomain"/> that implement the given step interface.
    /// Steps are registered in the order defined by the <see cref="PipelineStepAttribute.Order"/> attribute,
    /// using the <see cref="ServiceLifetime.Transient"/> service lifetime. Also registers an instance of
    /// <see cref="PipelineRunner{TContext, TStepInterface}"/> as <see cref="IPipelineRunner{TContext, TStepInterface}"/>,
    /// enabling resolution of the pipeline runner from the dependency injection container.
    /// </summary>
    /// <remarks>
    ///     <para>Specifying a filter allows for conditional registration of pipeline steps based on the filter.</para>
    ///     <list type="bullet">
    ///         <item>Steps without a filter will always be registered.</item>
    ///         <item>Steps with a filter will only be registered if the current filter contains a match for any of the filter constraints on the step.</item>
    ///     </list>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when the type <c>TContext</c> implements the generic <see cref="IPipelineStep"/> or <see cref="IPipelineRunner{TContext}"/> interface.
    /// </exception>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TStepInterface"></typeparam>
    /// <param name="services"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipeline<TContext, TStepInterface>(
        this IServiceCollection services,
        string[]? filters = null)
        where TContext : class
        where TStepInterface : IPipelineStep<TContext>
    {
        return services.AddPipeline<TContext, TStepInterface, IPipelineRunner<TContext, TStepInterface>>(
            AppDomain.CurrentDomain.GetAssemblies(),
            ServiceLifetime.Transient,
            filters);
    }

    /// <summary>
    /// Discovers and registers all pipeline steps in the <see cref="AppDomain.CurrentDomain"/> that implement the given step interface.
    /// Steps are registered in the order defined by the <see cref="PipelineStepAttribute.Order"/> attribute,
    /// using the specified <see cref="ServiceLifetime"/>. Also registers an instance of
    /// <see cref="PipelineRunner{TContext, TStepInterface}"/> as <see cref="IPipelineRunner{TContext, TStepInterface}"/>,
    /// enabling resolution of the pipeline runner from the dependency injection container.
    /// </summary>
    /// <remarks>
    ///     <para>Specifying a filter allows for conditional registration of pipeline steps based on the filter.</para>
    ///     <list type="bullet">
    ///         <item>Steps without a filter will always be registered.</item>
    ///         <item>Steps with a filter will only be registered if the current filter contains a match for any of the filter constraints on the step.</item>
    ///     </list>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when the type <c>TContext</c> implements the generic <see cref="IPipelineStep"/> or <see cref="IPipelineRunner{TContext}"/> interface.
    /// </exception>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TStepInterface"></typeparam>
    /// <param name="services"></param>
    /// <param name="lifetime"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipeline<TContext, TStepInterface>(
        this IServiceCollection services,
        ServiceLifetime lifetime,
        string[]? filters = null)
        where TContext : class
        where TStepInterface : IPipelineStep<TContext>
    {
        return services.AddPipeline<TContext, TStepInterface, IPipelineRunner<TContext, TStepInterface>>(
            AppDomain.CurrentDomain.GetAssemblies(),
            lifetime,
            filters);
    }

    /// <summary>
    /// Discovers and registers all pipeline steps in the specified assemblies that implement the given step interface.
    /// Steps are registered in the order defined by the <see cref="PipelineStepAttribute.Order"/> attribute,
    /// using the <see cref="ServiceLifetime.Transient"/> lifetime. Also registers an instance of
    /// <see cref="PipelineRunner{TContext, TStepInterface}"/> as <see cref="IPipelineRunner{TContext, TStepInterface}"/>,
    /// enabling resolution of the pipeline runner from the dependency injection container.
    /// </summary>
    /// <remarks>
    ///     <para>Specifying a filter allows for conditional registration of pipeline steps based on the filter.</para>
    ///     <list type="bullet">
    ///         <item>Steps without a filter will always be registered.</item>
    ///         <item>Steps with a filter will only be registered if the current filter contains a match for any of the filter constraints on the step.</item>
    ///     </list>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when the type <c>TContext</c> implements the generic <see cref="IPipelineStep"/> or <see cref="IPipelineRunner{TContext}"/> interface.
    /// </exception>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TStepInterface"></typeparam>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipeline<TContext, TStepInterface>(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        string[]? filters = null)
        where TContext : class
        where TStepInterface : IPipelineStep<TContext>
    {
        return services.AddPipeline<TContext, TStepInterface, IPipelineRunner<TContext, TStepInterface>>(
            assemblies,
            ServiceLifetime.Transient,
            filters);
    }

    /// <summary>
    /// Discovers and registers all pipeline steps in the specified assemblies that implement the given step interface.
    /// Steps are registered in the order defined by the <see cref="PipelineStepAttribute.Order"/> attribute,
    /// using the specified <see cref="ServiceLifetime"/>. Also registers an instance of
    /// <see cref="PipelineRunner{TContext, TStepInterface}"/> as <see cref="IPipelineRunner{TContext, TStepInterface}"/>,
    /// enabling resolution of the pipeline runner from the dependency injection container.
    /// </summary>
    /// <remarks>
    ///     <para>Specifying a filter allows for conditional registration of pipeline steps based on the filter.</para>
    ///     <list type="bullet">
    ///         <item>Steps without a filter will always be registered.</item>
    ///         <item>Steps with a filter will only be registered if the current filter contains a match for any of the filter constraints on the step.</item>
    ///     </list>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when the type <c>TContext</c> implements the generic <see cref="IPipelineStep"/> or <see cref="IPipelineRunner{TContext}"/> interface.
    /// </exception>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TStepInterface"></typeparam>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    /// <param name="lifetime"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipeline<TContext, TStepInterface>(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime lifetime,
        string[]? filters = null)
        where TContext : class
        where TStepInterface : IPipelineStep<TContext>
    {
        return services.AddPipeline<TContext, TStepInterface, IPipelineRunner<TContext, TStepInterface>>(
            assemblies,
            lifetime,
            filters);
    }

    #endregion

    #region AddPipeline<TContext, TStepInterface, TRunnerInterface>

    /// <summary>
    /// Discovers and registers all pipeline steps in the <see cref="AppDomain.CurrentDomain"/> that implement the given step interface.
    /// Steps are registered in the order defined by the <see cref="PipelineStepAttribute.Order"/> attribute,
    /// using the <see cref="ServiceLifetime.Transient"/> service lifetime. Also registers an instance of
    /// <see cref="PipelineRunner{TContext, TStepInterface}"/> as <typeparamref name="TRunnerInterface"/>,
    /// allowing for convenient resolution of the pipeline runner from the dependency injection container.
    /// </summary>
    /// <remarks>
    ///     <para>Specifying a filter allows for conditional registration of pipeline steps based on the filter.</para>
    ///     <list type="bullet">
    ///         <item>Steps without a filter will always be registered.</item>
    ///         <item>Steps with a filter will only be registered if the current filter contains a match for any of the filter constraints on the step.</item>
    ///     </list>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when the type <c>TContext</c> implements the generic <see cref="IPipelineStep"/> or <see cref="IPipelineRunner{TContext}"/> interface.
    /// </exception>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TStepInterface"></typeparam>
    /// <typeparam name="TRunnerInterface"></typeparam>
    /// <param name="services"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipeline<TContext, TStepInterface, TRunnerInterface>(
        this IServiceCollection services,
        string[]? filters = null)
        where TContext : class
        where TStepInterface : IPipelineStep<TContext>
        where TRunnerInterface : IPipelineRunner<TContext, TStepInterface>
    {
        return services.AddPipeline<TContext, TStepInterface, TRunnerInterface>(
            AppDomain.CurrentDomain.GetAssemblies(),
            ServiceLifetime.Transient,
            filters);
    }

    /// <summary>
    /// Discovers and registers all pipeline steps in the <see cref="AppDomain.CurrentDomain"/> that implement the given step interface.
    /// Steps are registered in the order defined by the <see cref="PipelineStepAttribute.Order"/> attribute,
    /// using the specified <see cref="ServiceLifetime"/>. Also registers an instance of
    /// <see cref="PipelineRunner{TContext, TStepInterface}"/> as <typeparamref name="TRunnerInterface"/>,
    /// allowing for convenient resolution of the pipeline runner from the dependency injection container.
    /// </summary>
    /// <remarks>
    ///     <para>Specifying a filter allows for conditional registration of pipeline steps based on the filter.</para>
    ///     <list type="bullet">
    ///         <item>Steps without a filter will always be registered.</item>
    ///         <item>Steps with a filter will only be registered if the current filter contains a match for any of the filter constraints on the step.</item>
    ///     </list>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when the type <c>TContext</c> implements the generic <see cref="IPipelineStep"/> or <see cref="IPipelineRunner{TContext}"/> interface.
    /// </exception>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TStepInterface"></typeparam>
    /// <typeparam name="TRunnerInterface"></typeparam>
    /// <param name="services"></param>
    /// <param name="lifetime"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipeline<TContext, TStepInterface, TRunnerInterface>(
        this IServiceCollection services,
        ServiceLifetime lifetime,
        string[]? filters = null)
        where TContext : class
        where TStepInterface : IPipelineStep<TContext>
        where TRunnerInterface : IPipelineRunner<TContext, TStepInterface>
    {
        return services.AddPipeline<TContext, TStepInterface, TRunnerInterface>(
            AppDomain.CurrentDomain.GetAssemblies(),
            lifetime,
            filters);
    }

    /// <summary>
    /// Discovers and registers all pipeline steps in the specified assemblies that implement the given step interface.
    /// Steps are registered in the order defined by the <see cref="PipelineStepAttribute.Order"/> attribute,
    /// using the <see cref="ServiceLifetime.Transient"/> service lifetime. Also registers an instance of
    /// <see cref="PipelineRunner{TContext, TStepInterface}"/> as <typeparamref name="TRunnerInterface"/>,
    /// allowing for convenient resolution of the pipeline runner from the dependency injection container.
    /// </summary>
    /// <remarks>
    ///     <para>Specifying a filter allows for conditional registration of pipeline steps based on the filter.</para>
    ///     <list type="bullet">
    ///         <item>Steps without a filter will always be registered.</item>
    ///         <item>Steps with a filter will only be registered if the current filter contains a match for any of the filter constraints on the step.</item>
    ///     </list>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when the type <c>TContext</c> implements the generic <see cref="IPipelineStep"/> or <see cref="IPipelineRunner{TContext}"/> interface.
    /// </exception>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TStepInterface"></typeparam>
    /// <typeparam name="TRunnerInterface"></typeparam>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipeline<TContext, TStepInterface, TRunnerInterface>(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        string[]? filters = null)
        where TContext : class
        where TStepInterface : IPipelineStep<TContext>
        where TRunnerInterface : IPipelineRunner<TContext, TStepInterface>
    {
        return services.AddPipeline<TContext, TStepInterface, TRunnerInterface>(
            assemblies,
            ServiceLifetime.Transient,
            filters);
    }

    /// <summary>
    /// Discovers and registers all pipeline steps in the specified assemblies that implement the given step interface.
    /// Steps are registered in the order defined by the <see cref="PipelineStepAttribute.Order"/> attribute,
    /// using the specified <see cref="ServiceLifetime"/>. Also registers an instance of
    /// <see cref="PipelineRunner{TContext, TStepInterface}"/> as <typeparamref name="TRunnerInterface"/>,
    /// allowing for convenient resolution of the pipeline runner from the dependency injection container.
    /// </summary>
    /// <remarks>
    ///     <para>Specifying a filter allows for conditional registration of pipeline steps based on the filter.</para>
    ///     <list type="bullet">
    ///         <item>Steps without a filter will always be registered.</item>
    ///         <item>Steps with a filter will only be registered if the current filter contains a match for any of the filter constraints on the step.</item>
    ///     </list>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when the type <c>TContext</c> implements the generic <see cref="IPipelineStep"/> or <see cref="IPipelineRunner{TContext}"/> interface.
    /// </exception>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TStepInterface"></typeparam>
    /// <typeparam name="TRunnerInterface"></typeparam>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    /// <param name="lifetime"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    public static IServiceCollection AddPipeline<TContext, TStepInterface, TRunnerInterface>(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime lifetime,
        string[]? filters = null)
        where TContext : class
        where TStepInterface : IPipelineStep<TContext>
        where TRunnerInterface : IPipelineRunner<TContext, TStepInterface>
    {
        // Calls the internal registration method to handle the actual registration logic.
        // This is the only place where the registration logic is defined, ensuring
        // that the same logic is used regardless of the overload called.
        return services.RegisterPipeline<TContext, TStepInterface, TRunnerInterface>(
            assemblies,
            lifetime,
            filters);
    }

    #endregion
}
