using System.Diagnostics.CodeAnalysis;

namespace PipeForge.Adapters.Diagnostics;

[ExcludeFromCodeCoverage]
internal sealed class NullScope : IPipelineDiagnosticsScope
{
    public static readonly NullScope Instance = new();
    public void Dispose() { }
    public void SetShortCircuited(bool value) { }
}
