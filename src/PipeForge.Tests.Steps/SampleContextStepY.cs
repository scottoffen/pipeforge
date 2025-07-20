namespace PipeForge.Tests.Steps;

[PipelineStep(20)]
public class SampleContextStepY : AbstractPipelineStep
{
    public static readonly string StepName = "Y";

    public SampleContextStepY()
    {
        Name = StepName;
    }
}
