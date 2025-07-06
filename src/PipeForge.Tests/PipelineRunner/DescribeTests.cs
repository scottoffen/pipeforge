using System.Text.Json;
using PipeForge.Tests.TestUtils;

namespace PipeForge.Tests.PipelineRunner;

public class DescribeTests
{
    private class NamedStep : IPipelineStep<TestContext>
    {
        public string Name { get; }
        public string? Description { get; }
        public bool MayShortCircuit { get; }
        public string? ShortCircuitCondition { get; }

        public NamedStep(string name, string? description = null, bool shortCircuiting = false, string? condition = null)
        {
            Name = name;
            Description = description;
            MayShortCircuit = shortCircuiting;
            ShortCircuitCondition = condition;
        }

        public Task InvokeAsync(TestContext context, PipelineDelegate<TestContext> next, CancellationToken cancellationToken = default)
        {
            return next(context, cancellationToken);
        }
    }

    [Fact]
    public void Describe_ReturnsStepDescriptions_InOrderAndWithMetadata()
    {
        // Arrange
        var steps = new List<Lazy<IPipelineStep<TestContext>>>
        {
            new(() => new NamedStep("StepA", "First step")),
            new(() => new NamedStep("StepB", "Second step", shortCircuiting: true, "stop if canceled")),
            new(() => new NamedStep("StepC", "Third step"))
        };

        var runner = new PipelineRunner<TestContext>(steps);

        // Act
        var json = runner.Describe();

        // Assert
        json.ShouldContain("\"Name\": \"StepA\"");
        json.ShouldContain("\"Name\": \"StepB\"");
        json.ShouldContain("\"Name\": \"StepC\"");

        json.ShouldContain("\"MayShortCircuit\": true");
        json.ShouldContain("\"ShortCircuitCondition\": \"stop if canceled\"");

        json.IndexOf("StepA").ShouldBeLessThan(json.IndexOf("StepB"));
        json.IndexOf("StepB").ShouldBeLessThan(json.IndexOf("StepC"));
    }

    [Fact]
    public void DescribeSchema_ReturnsValidJsonSchema()
    {
        // Arrange
        var runner = new PipelineRunner<TestContext>(Enumerable.Empty<Lazy<IPipelineStep<TestContext>>>());

        // Act
        var schema = runner.DescribeSchema();

        // Assert
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
        required.ShouldContain("Name");
        required.ShouldContain("MayShortCircuit");
    }
}
