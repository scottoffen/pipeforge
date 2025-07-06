using System.Threading;
using System.Threading.Tasks;

namespace PipeForge.Tests.Steps;

[PipelineStep(3, true, "1")]
public class StepC1 : PipelineStep<StepContext>
{
    public StepC1()
    {
        Name = "C1";
    }

    public override async Task InvokeAsync(StepContext context, PipelineDelegate<StepContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep(Name);
        await next(context, cancellationToken);
    }
}
