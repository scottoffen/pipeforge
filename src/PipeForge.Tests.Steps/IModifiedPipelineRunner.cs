namespace PipeForge.Tests.Steps;

public interface IModifiedPipelineRunner : IPipelineRunner<SampleContext, ISampleContextStep>
{
    int Id { get; }
}

public class ModifiedPipelineRunner : PipelineRunner<SampleContext, ISampleContextStep>, IModifiedPipelineRunner
{
    public static readonly int DefaultId = 35;

    public ModifiedPipelineRunner(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public int Id { get; } = DefaultId;
}
