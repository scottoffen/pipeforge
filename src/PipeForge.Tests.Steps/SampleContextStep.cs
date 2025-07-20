
namespace PipeForge.Tests.Steps;

public abstract class SampleContextStep : ISampleContextStep
{
    public string? Description { get; protected set; } = "Sample Description";

    public bool MayShortCircuit { get; protected set; } = false;

    public string Name { get; protected set; } = "Sample Name";

    public string? ShortCircuitCondition { get; protected set; } = null;

    public virtual Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep(Name);
        return next(context, cancellationToken);
    }
}
