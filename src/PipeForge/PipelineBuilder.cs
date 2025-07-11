using Microsoft.Extensions.Logging;

namespace PipeForge;

/// <summary>
/// PipelineBuilder is used to configure and build a pipeline of steps.
/// </summary>
/// <typeparam name="T"></typeparam>
public class PipelineBuilder<T>
{
    private readonly List<Lazy<IPipelineStep<T>>> _steps = new();
    private readonly ILoggerFactory? _loggerFactory;

    internal PipelineBuilder() { }

    internal PipelineBuilder(ILoggerFactory? loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Builds a pipeline from the configured steps
    /// </summary>
    /// <returns></returns>
    public IPipelineRunner<T> Build()
    {
        return new PipelineRunner<T>(_steps, _loggerFactory);
    }

    /// <summary>
    /// Adds a step to the pipeline
    /// </summary>
    /// <typeparam name="TStep"></typeparam>
    /// <returns></returns>
    public PipelineBuilder<T> WithStep<TStep>() where TStep : IPipelineStep<T>, new()
    {
        _steps.Add(new(() => new TStep()));
        return this;
    }

    /// <summary>
    /// Adds a step to the pipeline
    /// </summary>
    /// <typeparam name="TStep"></typeparam>
    /// <param name="stepFactory"></param>
    /// <returns></returns>
    public PipelineBuilder<T> WithStep<TStep>(Func<TStep> stepFactory) where TStep : IPipelineStep<T>
    {
        _steps.Add(new(() => stepFactory()));
        return this;
    }
}
