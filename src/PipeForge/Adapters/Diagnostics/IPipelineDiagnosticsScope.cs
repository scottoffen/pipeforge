namespace PipeForge.Adapters.Diagnostics;

internal interface IPipelineDiagnosticsScope : IDisposable
{
    /// <summary>
    /// Sets a tag for the diagnostics scope indicating that the pipeline was short-circuited.
    /// This is used to indicate that the pipeline execution was intentionally stopped before completion.
    /// </summary>
    /// <param name="value"></param>
    void SetShortCircuited(bool value);

    /// <summary>
    /// Sets a tag for the diagnostics scope indicating that the pipeline was canceled.
    /// This is used to indicate that the pipeline execution was canceled, either by user action or
    /// due to some other condition that prevents further processing.
    /// </summary>
    void SetCanceled();
}
