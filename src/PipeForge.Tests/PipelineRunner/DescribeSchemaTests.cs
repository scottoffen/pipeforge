using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PipeForge.Tests.Steps;

namespace PipeForge.Tests.PipelineRunner;

public class DescribeSchemaTests
{
   [Fact]
    public void DescribeSchema_ReturnsValidJsonSchema()
    {
        var provider = new ServiceCollection().BuildServiceProvider();
        var runner = new PipelineRunner<SampleContext>(provider);

        var schema = runner.DescribeSchema();

        var document = JsonDocument.Parse(schema);
        var root = document.RootElement;

        root.GetProperty("$schema").GetString().ShouldBe("http://json-schema.org/draft-07/schema#");
        root.GetProperty("title").GetString().ShouldBe("PipelineStep");
        root.GetProperty("type").GetString().ShouldBe("object");

        var properties = root.GetProperty("properties");
        properties.TryGetProperty("Order", out _).ShouldBeTrue();
        properties.TryGetProperty("Name", out _).ShouldBeTrue();
        properties.TryGetProperty("MayShortCircuit", out _).ShouldBeTrue();

        var required = root.GetProperty("required").EnumerateArray().Select(e => e.GetString()).ToArray();
        required.ShouldContain("Order");
    }
}
