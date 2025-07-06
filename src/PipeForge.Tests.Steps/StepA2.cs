using System.Threading;
using System.Threading.Tasks;

namespace PipeForge.Tests.Steps;

[PipelineStep(1, "2")]
public class StepA2 : PipelineStep<StepContext>
{
    public StepA2()
    {
        Name = "A2";
    }

    public override async Task InvokeAsync(StepContext context, PipelineDelegate<StepContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep(Name);
        await next(context, cancellationToken);
    }
}
