using System.Diagnostics.CodeAnalysis;

namespace PipeForge.Adapters.Diagnostics;

internal interface IPipelineDiagnostics<T> where T : class
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

[ExcludeFromCodeCoverage]
internal sealed class NullScope : IPipelineDiagnosticsScope
{
    public static readonly NullScope Instance = new();
    public void Dispose() { }

    public void SetCanceled() {}

    public void SetShortCircuited(bool value) { }
}
