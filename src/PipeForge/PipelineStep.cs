using System.Diagnostics.CodeAnalysis;

namespace PipeForge;

[ExcludeFromCodeCoverage]
public abstract class PipelineStep<TContext> : IPipelineStep<TContext>
    where TContext : class
{
    public virtual string? Description { get; set; } = null;

    public virtual bool MayShortCircuit { get; set; }

    public virtual string Name { get; set; } = string.Empty;

    public virtual string? ShortCircuitCondition { get; set; } = null;

    public abstract Task InvokeAsync(TContext context, PipelineDelegate<TContext> next, CancellationToken cancellationToken = default);
}
