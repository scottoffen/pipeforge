using System.Reflection;

namespace PipeForge.Tests;

public class PipelineStepAttributeTests
{
    [Fact]
    public void Constructor_Assigns_Properties_Correctly()
    {
        var attr = new PipelineStepAttribute(order: 42, environment: "Staging");

        attr.Order.ShouldBe(42);
        attr.Environment.ShouldBe("Staging");
    }

    [Fact]
    public void Defaults_Environment_To_Null()
    {
        var attr = new PipelineStepAttribute(order: 1);

        attr.Environment.ShouldBeNull();
    }

    [Fact]
    public void Attribute_Can_Be_Applied_To_Class_Only()
    {
        var usage = typeof(PipelineStepAttribute)
            .GetCustomAttribute<AttributeUsageAttribute>();

        usage.ShouldNotBeNull();
        usage.ValidOn.ShouldBe(AttributeTargets.Class);
        usage.Inherited.ShouldBeFalse();
        usage.AllowMultiple.ShouldBeFalse();
    }
}
