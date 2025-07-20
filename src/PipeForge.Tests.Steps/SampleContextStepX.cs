namespace PipeForge.Tests.Steps;

[PipelineStep(10)]
public class SampleContextStepX : AbstractPipelineStep
{
    public static readonly string StepName = "X";

    public SampleContextStepX()
    {
        Name = StepName;
    }
}
