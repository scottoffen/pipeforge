using PipeForge.Tests.NetCoreApp.TestUtils;

namespace PipeForge.Tests.NetCoreApp.PipelineRunner;

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
}
