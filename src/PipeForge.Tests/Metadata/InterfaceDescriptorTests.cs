using PipeForge.Metadata;
using PipeForge.Tests.Steps;

namespace PipeForge.Tests.Metadata;

public class InterfaceDescriptorTests
{
    [Fact]
    public void InterfaceType_ReturnsCorrectInterface()
    {
        InterfaceDescriptor<IPipelineStep<SampleContext>>.InterfaceType
            .ShouldBe(typeof(IPipelineStep<SampleContext>));

        InterfaceDescriptor<ISampleContextStep>.InterfaceType
            .ShouldBe(typeof(ISampleContextStep));
    }

    [Fact]
    public void LazyType_ReturnsCorrectLazyType()
    {
        InterfaceDescriptor<IPipelineStep<SampleContext>>.LazyType
            .ShouldBe(typeof(Lazy<IPipelineStep<SampleContext>>));

        InterfaceDescriptor<ISampleContextStep>.LazyType
            .ShouldBe(typeof(Lazy<ISampleContextStep>));
    }

    [Fact]
    public void StaticConstructor_ThrowsArgumentException_WhenNotInterface()
    {
        var ex = Should.Throw<TypeInitializationException>(() =>
        {
            _ = InterfaceDescriptor<SampleContext>.InterfaceType;
        });

        ex.InnerException.ShouldBeOfType<ArgumentException>();
        ex.InnerException!.Message.ShouldContain("is not an interface");
    }
}
