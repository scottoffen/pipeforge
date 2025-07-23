namespace PipeForge.Tests.Steps;

[PipelineStep(100)]
public class SampleContextStepZ : SampleContextStep
{
    public static readonly string StepName = "Z";

    public SampleContextStepZ()
    {
        Name = StepName;
    }
}
