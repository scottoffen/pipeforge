namespace PipeForge.Tests.Steps;

[PipelineStep(200, TestConstants.Filter1, TestConstants.Filter2)]
public class SampleContextStepM : AbstractPipelineStep
{
    public static readonly string StepName = "M";

    public SampleContextStepM()
    {
        Name = StepName;
    }
}
