using PipeForge.Tests.TestUtils;

namespace PipeForge.Tests.PipelineRunner;

public class ExceptionHandlingTests
{
    private class ThrowingStep : IPipelineStep<TestContext>
    {
        public string Name { get; }
        public string? Description => "Always throws";
        public bool MayShortCircuit => false;
        public string? ShortCircuitCondition => null;

        public ThrowingStep(string name) => Name = name;

        public Task InvokeAsync(TestContext context, PipelineDelegate<TestContext> next, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Step failed.");
        }
    }

    private class PreWrappedExceptionStep : IPipelineStep<TestContext>
    {
        public string Name { get; }
        public string? Description => "Throws pre-wrapped";
        public bool MayShortCircuit => false;
        public string? ShortCircuitCondition => null;

        public PreWrappedExceptionStep(string name) => Name = name;

        public Task InvokeAsync(TestContext context, PipelineDelegate<TestContext> next, CancellationToken cancellationToken = default)
        {
            throw new PipelineExecutionException<TestContext>(Name, 0, new Exception("Already wrapped."));
        }
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsWrappedException_WhenStepThrows()
    {
        // Arrange
        var steps = new List<Lazy<IPipelineStep<TestContext>>>
        {
            new(() => new ThrowingStep("FailingStep"))
        };

        var runner = new PipelineRunner<TestContext>(steps);

        // Act
        var ex = await Should.ThrowAsync<PipelineExecutionException<TestContext>>(
            () => runner.ExecuteAsync(new TestContext()));

        // Assert
        ex.StepName.ShouldBe("FailingStep");
        ex.StepOrder.ShouldBe(0);
        ex.InnerException.ShouldBeOfType<InvalidOperationException>();
        ex.InnerException?.Message.ShouldBe("Step failed.");
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotWrap_PipelineExecutionException()
    {
        // Arrange
        var steps = new List<Lazy<IPipelineStep<TestContext>>>
        {
            new(() => new PreWrappedExceptionStep("WrappedStep"))
        };

        var runner = new PipelineRunner<TestContext>(steps);

        // Act & Assert
        var ex = await Should.ThrowAsync<PipelineExecutionException<TestContext>>(
            () => runner.ExecuteAsync(new TestContext()));

        ex.StepName.ShouldBe("WrappedStep");
        ex.StepOrder.ShouldBe(0);
        ex.InnerException?.Message.ShouldBe("Already wrapped.");
    }
}
