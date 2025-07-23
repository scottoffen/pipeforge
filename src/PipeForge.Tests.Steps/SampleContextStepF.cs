namespace PipeForge.Tests.Steps;

[PipelineStep(4, TestConstants.Filter1)]
public class SampleContextStepF1 : SampleContextStep
{
    public static readonly string StepName = "F";

    public SampleContextStepF1()
    {
        Name = StepName;
    }

    public override Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep(Name);
        return Task.CompletedTask;
    }
}

[PipelineStep(4, TestConstants.Filter2)]
public class SampleContextStepF2 : SampleContextStep
{
    public static readonly string StepName = "F";

    public SampleContextStepF2()
    {
        Name = StepName;
    }

    public override Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("This step should not be executed in this test.");
    }
}
