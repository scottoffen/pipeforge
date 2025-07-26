using Microsoft.Extensions.DependencyInjection;

namespace PipeForge;

/// <summary>
/// PipelineBuilder is used to configure and build a pipeline of steps.
/// </summary>
/// <typeparam name="TContext"></typeparam>
public class PipelineBuilder<TContext>
    where TContext : class
{
    private readonly IServiceCollection _services = new ServiceCollection();

    internal PipelineBuilder()
    { }

    /// <summary>
    /// Builds a pipeline from the configured steps
    /// </summary>
    /// <returns></returns>
    public IPipelineRunner<TContext> Build()
    {
        return new PipelineRunner<TContext>(_services.BuildServiceProvider());
        throw new NotImplementedException();
    }

    /// <summary>
    /// Configures the services used by the pipeline
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public PipelineBuilder<TContext> ConfigureServices(Action<IServiceCollection> configure)
    {
        configure(_services);
        return this;
    }

    /// <summary>
    /// Adds a step to the pipeline
    /// </summary>
    /// <typeparam name="TStep"></typeparam>
    /// <returns></returns>
    public PipelineBuilder<TContext> WithStep<TStep>(ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TStep : class, IPipelineStep<TContext>
    {
        _services.AddPipelineStep<TStep>(lifetime);
        return this;
    }

    /// <summary>
    /// Adds a step to the pipeline using a delegate. By default, the step is registered with a transient lifetime.
    /// You can specify a different lifetime if needed.
    /// </summary>
    /// <param name="invoke">The delegate to invoke for the step.</param>
    /// <param name="lifetime">The service lifetime for the step. Defaults to <see cref="ServiceLifetime.Transient"/>.</param>
    /// <returns></returns>
    public PipelineBuilder<TContext> WithStep(
        Func<TContext, PipelineDelegate<TContext>, CancellationToken, Task> invoke,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        var stepFactory = new Func<IServiceProvider, DelegatePipelineStep<TContext>>(_ => new DelegatePipelineStep<TContext>(invoke));

        _services.Add(ServiceDescriptor.Describe(typeof(IPipelineStep<TContext>), stepFactory, lifetime));
        _services.Add(ServiceDescriptor.Describe(typeof(Lazy<IPipelineStep<TContext>>),
            sp => new Lazy<IPipelineStep<TContext>>(() => stepFactory(sp)), lifetime));

        return this;
    }
}
