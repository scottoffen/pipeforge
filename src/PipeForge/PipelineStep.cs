
using System.Diagnostics.CodeAnalysis;

namespace PipeForge;

[ExcludeFromCodeCoverage]
public abstract class PipelineStep<T> : IPipelineStep<T>
{
    public string? Description { get; set; } = null;

    public bool MayShortCircuit { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? ShortCircuitCondition { get; set; } = null;

    public abstract Task InvokeAsync(T context, PipelineDelegate<T> next, CancellationToken cancellationToken = default);
}
