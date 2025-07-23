using PipeForge.Extensions;
using PipeForge.Tests.Steps;

namespace PipeForge.Tests.Extensions;

public class TypeExtensionsTests
{
    private interface ICustom { }

    private class DerivedWithoutInterface : ImplementsDirectly { }

    private class ImplementsDirectly : ICustom { }

    [Fact]
    public void ImplementsPipelineStep_ReturnsFalse_WhenTypeIsNull()
    {
        Type? type = null;

        var result = type!.ImplementsPipelineStep();

        result.ShouldBeFalse();
    }

    [Fact]
    public void ImplementsPipelineStep_ReturnsTrue_WhenTypeImplementsInterfaceDirectly()
    {
        var type = typeof(ISampleContextStep);

        var result = type.ImplementsPipelineStep();

        result.ShouldBeTrue();
    }

    [Fact]
    public void ImplementsPipelineStep_ReturnsTrue_WhenTypeImplementsOpenGenericVariant()
    {
        var type = typeof(IGenericPipelineStep<>);

        var result = type.ImplementsPipelineStep();

        result.ShouldBeTrue();
    }

    [Fact]
    public void ImplementsPipelineStep_ReturnsTrue_WhenTypeInheritsFromTypeThatImplementsInterface()
    {
        var type = typeof(SampleContextStepA);

        var result = type.ImplementsPipelineStep();

        result.ShouldBeTrue();
    }

    [Fact]
    public void ImplementsPipelineStep_ReturnsFalse_WhenTypeDoesNotImplement()
    {
        var type = typeof(string);

        var result = type.ImplementsPipelineStep();

        result.ShouldBeFalse();
    }

    [Fact]
    public void DirectlyImplements_ReturnsTrue_WhenTypeImplementsInterfaceAndBaseDoesNot()
    {
        var type = typeof(ImplementsDirectly);
        var result = type.DirectlyImplements(typeof(ICustom));
        result.ShouldBeTrue();
    }

    [Fact]
    public void DirectlyImplements_ReturnsFalse_WhenInterfaceIsImplementedByBase()
    {
        var type = typeof(DerivedWithoutInterface);
        var result = type.DirectlyImplements(typeof(ICustom));
        result.ShouldBeFalse();
    }

    [Fact]
    public void DirectlyImplements_ReturnsFalse_WhenInterfaceIsNotImplementedAtAll()
    {
        var type = typeof(string);
        var result = type.DirectlyImplements(typeof(ICustom));
        result.ShouldBeFalse();
    }
}
