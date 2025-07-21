using PipeForge.Extensions;
using PipeForge.Tests.Steps;

namespace PipeForge.Tests.Extensions;

public class InternalTypeExtensionsTests
{
    private class InternalStep : PipelineStep<SampleContext>
    {
        public override Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    private class OpenGenericStep<T> : PipelineStep<T>
    {
        public override Task InvokeAsync(T context, PipelineDelegate<T> next, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public void IsPipelineStep_ReturnsTrue_WhenTypeIsValidPipelineStep()
    {
        typeof(InternalStep).IsPipelineStep<IPipelineStep<SampleContext>>().ShouldBeTrue();
        typeof(SampleContextStepA).IsPipelineStep<ISampleContextStep>().ShouldBeTrue();
    }

    [Fact]
    public void IsPipelineStep_ReturnsFalse_WhenTypeIsNotAClass()
    {
        typeof(ISampleContextStep).IsPipelineStep<IPipelineStep<SampleContext>>().ShouldBeFalse();
    }

    [Fact]
    public void IsPipelineStep_ReturnsFalse_WhenTypeIsAbstract()
    {
        typeof(SampleContextStep).IsPipelineStep<IPipelineStep<SampleContext>>().ShouldBeFalse();
    }

    [Fact]
    public void IsPipelineStep_ReturnsFalse_WhenTypeIsOpenGenericTypeDefinition()
    {
        typeof(OpenGenericStep<>).IsPipelineStep<IPipelineStep<SampleContext>>().ShouldBeFalse();
    }

    [Fact]
    public void IsPipelineStep_ReturnsFalse_WhenTypeIsNotAssignableToInterface()
    {
        typeof(InternalStep).IsPipelineStep<ISampleContextStep>().ShouldBeFalse();
    }

    [Fact]
    public void IsPipelineStep_ReturnsFalse_WhenInterfaceIsNotIPipelineStep()
    {
        typeof(ModifiedPipelineRunner).IsPipelineStep<IModifiedPipelineRunner>().ShouldBeFalse();
    }

    [Fact]
    public void ImplementsPipelineStep_ReturnsTrue_WhenTypeImplementsGenericInterface()
    {
        typeof(SampleContextStepA).ImplementsPipelineStep().ShouldBeTrue();
    }

    [Fact]
    public void ImplementsPipelineStep_ReturnsTrue_WhenTypeIsGenericDefinitionOfInterface()
    {
        typeof(IPipelineStep<>).ImplementsPipelineStep().ShouldBeTrue();
    }

    [Fact]
    public void ImplementsPipelineStep_ReturnsFalse_WhenTypeDoesNotMatch()
    {
        typeof(string).ImplementsPipelineStep().ShouldBeFalse();
    }

    [Fact]
    public void ImplementsPipelineStep_ReturnsFalse_WhenTypeIsNull()
    {
        Type? type = null;
        type.ImplementsPipelineStep().ShouldBeFalse();
    }
}
