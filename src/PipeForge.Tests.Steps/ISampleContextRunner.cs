namespace PipeForge.Tests.Steps;

public interface ISampleContextRunner : IPipelineRunner<SampleContext, ISampleContextStep>
{ }

public class SampleContextRunner : PipelineRunner<SampleContext, ISampleContextStep>, ISampleContextRunner
{
    public SampleContextRunner(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
