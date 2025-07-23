namespace PipeForge.Tests.Steps;

[PipelineStep(3)]
public class SampleContextStepB : SampleContextStep
{
    public static readonly string StepName = "B";

    public SampleContextStepB()
    {
        Name = StepName;
    }
}
