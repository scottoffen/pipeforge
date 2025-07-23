namespace PipeForge.Tests.Steps;

public interface IGenericPipelineStep<T> : IPipelineStep<T> where T : class
{
}

public class GenericPipelineStep<T> : IGenericPipelineStep<T> where T : class
{
    public string? Description => throw new NotImplementedException();

    public bool MayShortCircuit => throw new NotImplementedException();

    public string Name => throw new NotImplementedException();

    public string? ShortCircuitCondition => throw new NotImplementedException();

    public Task InvokeAsync(T context, PipelineDelegate<T> next, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

[PipelineStep(0)]
public class ClosedGenericPipelineStep : GenericPipelineStep<SampleContext>, IGenericPipelineStep<SampleContext>
{
    // This class is a closed generic implementation of IGenericPipelineStep<SampleContext>.
}

public class OpenGenericPipelineStep<T> : ClosedGenericPipelineStep
{
}
