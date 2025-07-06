using System.Threading;
using System.Threading.Tasks;

namespace PipeForge.Tests.Steps;

[PipelineStep(3, true, "2")]
public class StepB2 : PipelineStep<StepContext>
{
    public StepB2()
    {
        Name = "B2";
    }

    public override async Task InvokeAsync(StepContext context, PipelineDelegate<StepContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep(Name);
        await next(context, cancellationToken);
    }
}
