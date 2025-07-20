namespace PipeForge.Tests.Steps;

[PipelineStep(1)]
public class SampleContextStepA : SampleContextStep
{
    public static readonly string StepName = "A";

    public SampleContextStepA()
    {
        Name = StepName;
    }
}
