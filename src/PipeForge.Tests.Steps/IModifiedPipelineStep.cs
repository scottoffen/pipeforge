namespace PipeForge.Tests.Steps;

public interface IModifiedPipelineStep<T> : IPipelineStep<T>
{
    bool IsModified { get; set; }
}
