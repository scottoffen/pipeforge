using System.Threading;
using System.Threading.Tasks;

namespace PipeForge.Tests.Steps;

[PipelineStep(1, "1")]
public class StepA1 : PipelineStep<StepContext>
{
    public StepA1()
    {
        Name = "A1";
    }

    public override async Task InvokeAsync(StepContext context, PipelineDelegate<StepContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep(Name);
        await next(context, cancellationToken);
    }
}
