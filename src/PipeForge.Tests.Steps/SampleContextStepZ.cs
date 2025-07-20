namespace PipeForge.Tests.Steps;

[PipelineStep(30)]
public class SampleContextStepZ : AbstractPipelineStep
{
    public static readonly string StepName = "Z";

    public SampleContextStepZ()
    {
        Name = StepName;
    }
}
