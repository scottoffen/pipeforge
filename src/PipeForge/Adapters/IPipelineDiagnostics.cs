namespace PipeForge.Adapters;

internal interface IPipelineDiagnostics<T>
{
    /// <summary>
    /// Begins a diagnostics scope for the given step.
    /// </summary>
    /// <param name="step"></param>
    /// <param name="order"></param>
    /// <returns></returns>
    IPipelineDiagnosticsScope BeginStep(IPipelineStep<T> step, int order);

    /// <summary>
    /// Reports an exception that occurred during the execution of a step in the pipeline.
    /// </summary>
    /// <param name="ex"></param>
    /// <param name="step"></param>
    /// <param name="order"></param>
    void ReportException(Exception ex, IPipelineStep<T> step, int order);
}

internal interface IPipelineDiagnosticsScope : IDisposable
{
    /// <summary>
    /// Sets a tag for the diagnostics scope.
    /// </summary>
    /// <param name="value"></param>
    void SetShortCircuited(bool value);
}

internal static class PipelineDiagnosticsFactory
{
    /// <summary>
    /// Returns an implementation of IPipelineDiagnostics for the given type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IPipelineDiagnostics<T> Create<T>()
    {
#if NETSTANDARD2_0
        return new DiagnosticListenerProvider<T>();
#else
        return new ActivitySourceProvider<T>();
#endif
    }
}

internal sealed class NullScope : IPipelineDiagnosticsScope
{
    public static readonly NullScope Instance = new();
    public void Dispose() { }
    public void SetShortCircuited(bool value) { }
}
