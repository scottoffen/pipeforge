using System.Diagnostics.CodeAnalysis;

namespace PipeForge;

[ExcludeFromCodeCoverage]
public abstract class PipelineStep<TContext> : IPipelineStep<TContext>
    where TContext : class
{
    public string? Description { get; set; } = null;

    public bool MayShortCircuit { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? ShortCircuitCondition { get; set; } = null;

    public abstract Task InvokeAsync(TContext context, PipelineDelegate<TContext> next, CancellationToken cancellationToken = default);
}
