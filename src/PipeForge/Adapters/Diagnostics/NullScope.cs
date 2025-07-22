using System.Diagnostics.CodeAnalysis;

namespace PipeForge.Adapters.Diagnostics;

[ExcludeFromCodeCoverage]
internal sealed class NullScope : IPipelineDiagnosticsScope
{
    public static readonly NullScope Instance = new();

    private NullScope() { }

    public void Dispose() { }

    public void SetCanceled() { }

    public void SetShortCircuited(bool value) { }
}
