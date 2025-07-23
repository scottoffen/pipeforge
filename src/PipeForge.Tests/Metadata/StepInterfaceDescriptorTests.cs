using PipeForge.Metadata;
using PipeForge.Tests.Steps;

namespace PipeForge.Tests.Metadata;

public class StepInterfaceDescriptorTests
{
    private class NotAnInterface { }

    private interface INotAStep { }

    [Fact]
    public void StepInterfaceDescriptor_ThrowsArgumentException_WhenTypeIsNotAnInterface()
    {
        var ex = Should.Throw<TypeInitializationException>(() =>
        {
            _ = StepInterfaceDescriptor<NotAnInterface>.InterfaceType;
        });

        ex.InnerException.ShouldBeOfType<ArgumentException>();
        ex.InnerException!.Message.ShouldContain("is not an interface");
    }

    [Fact]
    public void StepInterfaceDescriptor_ThrowsArgumentException_WhenTypeIsNotAPipelineStep()
    {
        var ex = Should.Throw<TypeInitializationException>(() =>
        {
            _ = StepInterfaceDescriptor<INotAStep>.InterfaceType;
        });

        ex.InnerException.ShouldBeOfType<ArgumentException>();
        ex.InnerException!.Message.ShouldContain("does not implement the IPipelineStep interface");
    }

    [Fact]
    public void StepInterfaceDescriptor_InitializesSuccessfully_WhenValidPipelineStepInterfaceIsProvided()
    {
        StepInterfaceDescriptor<ISampleContextStep>.InterfaceType.ShouldBe(typeof(ISampleContextStep));
        StepInterfaceDescriptor<ISampleContextStep>.LazyType.ShouldBe(typeof(Lazy<ISampleContextStep>));
    }
}
