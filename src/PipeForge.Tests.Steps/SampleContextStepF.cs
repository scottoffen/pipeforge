namespace PipeForge.Tests.Steps;

[PipelineStep(100, TestConstants.Filter1)]
public class SampleContextStepF : AbstractPipelineStep
{
    public static readonly string StepName = "F";

    public SampleContextStepF()
    {
        Name = StepName;
    }
}
