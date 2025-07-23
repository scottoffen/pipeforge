using PipeForge.Metadata;
using PipeForge.Tests.Steps;

namespace PipeForge.Tests.Metadata;

public class StepTypeDescriptorTests
{
    private interface INotAStep { }

    private class ImplementsNothing { }

    private class ImplementsINotAStep : INotAStep { }

    [Fact]
    public void CreateGeneric_ThrowsArgumentException_WhenTypeDoesNotImplementInterface()
    {
        var ex = Should.Throw<ArgumentException>(() =>
        {
            StepTypeDescriptor.Create<ISampleContextStep>(typeof(ImplementsNothing));
        });

        ex.Message.ShouldContain("does not implement the interface");
        ex.ParamName.ShouldBeNull(); // because we used string.Format, not constructor with paramName
    }

    [Fact]
    public void CreateGeneric_ReturnsDescriptor_WhenTypeImplementsInterface()
    {
        var result = StepTypeDescriptor.Create<ISampleContextStep>(typeof(SampleContextStepA));

        result.ConcreteType.ShouldBe(typeof(SampleContextStepA));
        result.InterfaceType.ShouldBe(typeof(ISampleContextStep));
        result.LazyType.ShouldBe(typeof(Lazy<ISampleContextStep>));
        result.TypeName.ShouldBe(typeof(SampleContextStepA).FullName);
    }

    [Fact]
    public void CreateNonGeneric_ThrowsArgumentException_WhenNoPipelineStepInterfaceFound()
    {
        var ex = Should.Throw<ArgumentException>(() =>
        {
            StepTypeDescriptor.Create(typeof(ImplementsINotAStep));
        });

        ex.Message.ShouldContain("does not implement the interface");
    }

    [Fact]
    public void CreateNonGeneric_ReturnsDescriptor_WhenPipelineStepInterfaceIsImplemented()
    {
        var result = StepTypeDescriptor.Create(typeof(SampleContextStepA));

        result.ConcreteType.ShouldBe(typeof(SampleContextStepA));
        result.InterfaceType.ShouldBe(typeof(IPipelineStep<SampleContext>));
        result.LazyType.ShouldBe(typeof(Lazy<IPipelineStep<SampleContext>>));
        result.TypeName.ShouldBe(typeof(SampleContextStepA).FullName);
    }

    [Fact]
    public void CreateNonGeneric_ReturnsCachedLazyType_WhenCalledMultipleTimes()
    {
        var first = StepTypeDescriptor.Create(typeof(SampleContextStepA));
        var second = StepTypeDescriptor.Create(typeof(SampleContextStepA));

        first.LazyType.ShouldBeSameAs(second.LazyType);
    }
}
