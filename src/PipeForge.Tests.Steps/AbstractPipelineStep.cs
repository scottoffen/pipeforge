
namespace PipeForge.Tests.Steps;

public class AbstractPipelineStep : PipelineStep<SampleContext>
{
    public override Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep(Name);
        return Task.CompletedTask;
    }
}
