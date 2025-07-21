using PipeForge.Metadata;
using PipeForge.Tests.Steps;

namespace PipeForge.Tests.Metadata;

public class TypeDescriptorTests
{
    [Fact]
    public void Create_WithExplicitInterface_SetsAllProperties()
    {
        var descriptor = TypeDescriptor.Create<ISampleContextStep>(typeof(SampleContextStepA));

        descriptor.ConcreteType.ShouldBe(typeof(SampleContextStepA));
        descriptor.InterfaceType.ShouldBe(typeof(ISampleContextStep));
        descriptor.LazyType.ShouldBe(typeof(Lazy<ISampleContextStep>));
        descriptor.TypeName.ShouldBe(typeof(SampleContextStepA).FullName);
    }

    [Fact]
    public void Create_WithExplicitInterface_Throws_WhenTypeDoesNotImplementInterface()
    {
        var ex = Should.Throw<ArgumentException>(() =>
        {
            _ = TypeDescriptor.Create<IPipelineStep<SampleContext>>(typeof(string));
        });

        ex.Message.ShouldContain("does not implement the interface");
    }

    [Fact]
    public void Create_WithGenericDetection_SetsAllProperties()
    {
        var descriptor = TypeDescriptor.Create(typeof(SampleContextStepA));

        descriptor.ConcreteType.ShouldBe(typeof(SampleContextStepA));
        descriptor.InterfaceType.ShouldBe(typeof(IPipelineStep<SampleContext>));
        descriptor.LazyType.ShouldBe(typeof(Lazy<IPipelineStep<SampleContext>>));
        descriptor.TypeName.ShouldBe(typeof(SampleContextStepA).FullName);
    }

    [Fact]
    public void Create_WithGenericDetection_Throws_WhenTypeDoesNotImplementGenericInterface()
    {
        var ex = Should.Throw<ArgumentException>(() =>
        {
            _ = TypeDescriptor.Create(typeof(string));
        });

        ex.Message.ShouldContain("does not implement the interface");
    }
}
