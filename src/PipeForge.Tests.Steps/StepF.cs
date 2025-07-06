using System.Threading;
using System.Threading.Tasks;

namespace PipeForge.Tests.Steps;

public class StepF : PipelineStep<StepContext>
{
    public StepF()
    {
        Name = "FX";
    }

    public override async Task InvokeAsync(StepContext context, PipelineDelegate<StepContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep(Name);
        await next(context, cancellationToken);
    }
}
