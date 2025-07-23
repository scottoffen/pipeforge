namespace PipeForge.Tests.Steps;

[PipelineStep(4, TestConstants.Filter1, TestConstants.Filter2)]
public class SampleContextStepM : SampleContextStep
{
    public static readonly string StepName = "M";

    public SampleContextStepM()
    {
        Name = StepName;
    }
}
