using PipeForge.Tests.TestUtils;

namespace PipeForge.Tests.PipelineRunner;

public class ContextHandlingTests
{
    private class TrackingStep : IPipelineStep<TestContext>
    {
        public string Name => "Tracking";
        public string? Description => null;
        public bool MayShortCircuit => false;
        public string? ShortCircuitCondition => null;

        public TestContext? InvokedWith { get; private set; }

        public Task InvokeAsync(TestContext context, PipelineDelegate<TestContext> next, CancellationToken cancellationToken = default)
        {
            InvokedWith = context;
            return next(context, cancellationToken);
        }
    }

    [Fact]
    public async Task NullContext_ThrowsArgumentNullException()
    {
        var runner = new PipelineRunner<TestContext>(Enumerable.Empty<Lazy<IPipelineStep<TestContext>>>());

        var ex = await Should.ThrowAsync<ArgumentNullException>(() => runner.ExecuteAsync(null!));

        ex.ParamName.ShouldBe("context");
    }

    [Fact]
    public async Task ValidContext_IsPassedToSteps()
    {
        var context = new TestContext();
        var steps = new List<TrackingStep>
        {
            new TrackingStep(),
            new TrackingStep()
        };

        var lazySteps = steps
            .Select(step => new Lazy<IPipelineStep<TestContext>>(() => step))
            .ToList();

        var runner = new PipelineRunner<TestContext>(lazySteps);

        await runner.ExecuteAsync(context);

        foreach (var step in steps)
        {
            step.InvokedWith.ShouldBe(context);
        }
    }
}
