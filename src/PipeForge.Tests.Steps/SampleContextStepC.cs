namespace PipeForge.Tests.Steps;

[PipelineStep(2)]
public class SampleContextStepC : SampleContextStep
{
    public static readonly string StepName = "C";

    public SampleContextStepC()
    {
        Name = StepName;
    }
}
