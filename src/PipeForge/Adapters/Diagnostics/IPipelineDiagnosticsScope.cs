namespace PipeForge.Adapters.Diagnostics;

internal interface IPipelineDiagnosticsScope : IDisposable
{
    /// <summary>
    /// Sets a tag for the diagnostics scope.
    /// </summary>
    /// <param name="value"></param>
    void SetShortCircuited(bool value);
}
