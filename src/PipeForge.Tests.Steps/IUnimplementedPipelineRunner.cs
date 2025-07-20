namespace PipeForge.Tests.Steps;

public interface IUnimplementedPipelineRunner : IPipelineRunner<SampleContext, ISampleContextStep>
{
    int Id { get; }
}

