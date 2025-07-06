using PipeForge.Tests.TestUtils;

namespace PipeForge.Tests.PipelineRunner;

public class ExecutionFlowTests
{
    private class StepA : IPipelineStep<TestContext>
    {
        public string Name => "StepA";
        public string? Description => null;
        public bool MayShortCircuit => false;
        public string? ShortCircuitCondition => null;

        public Task InvokeAsync(TestContext context, PipelineDelegate<TestContext> next, CancellationToken cancellationToken = default)
        {
            context.ExecutedSteps.Add("A");
            return next(context, cancellationToken);
        }
    }

    private class StepB : IPipelineStep<TestContext>
    {
        public string Name => "StepB";
        public string? Description => null;
        public bool MayShortCircuit => false;
        public string? ShortCircuitCondition => null;

        public Task InvokeAsync(TestContext context, PipelineDelegate<TestContext> next, CancellationToken cancellationToken = default)
        {
            context.ExecutedSteps.Add("B");
            return next(context, cancellationToken);
        }
    }

    private class StepShortCircuit : IPipelineStep<TestContext>
    {
        public string Name => "StepShortCircuit";
        public string? Description => null;
        public bool MayShortCircuit => true;
        public string? ShortCircuitCondition => "Always short-circuits";

        public Task InvokeAsync(TestContext context, PipelineDelegate<TestContext> next, CancellationToken cancellationToken = default)
        {
            context.ExecutedSteps.Add("ShortCircuit");
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Executes_All_Steps_In_Order()
    {
        var context = new TestContext();
        var steps = new List<Lazy<IPipelineStep<TestContext>>>
        {
            new(() => new StepA()),
            new(() => new StepB())
        };

        var runner = new PipelineRunner<TestContext>(steps);
        await runner.ExecuteAsync(context);

        context.ExecutedSteps.ShouldBe(new[] { "A", "B" });
    }

    [Fact]
    public async Task Stops_Execution_When_ShortCircuiting()
    {
        var context = new TestContext();
        var steps = new List<Lazy<IPipelineStep<TestContext>>>
        {
            new(() => new StepA()),
            new(() => new StepShortCircuit()),
            new(() => new StepB()) // should not run
        };

        var runner = new PipelineRunner<TestContext>(steps);
        await runner.ExecuteAsync(context);

        context.ExecutedSteps.ShouldBe(new[] { "A", "ShortCircuit" });
    }

    [Fact]
    public async Task Executes_No_Steps_When_Pipeline_Is_Empty()
    {
        var context = new TestContext();
        var runner = new PipelineRunner<TestContext>(Enumerable.Empty<Lazy<IPipelineStep<TestContext>>>());

        await runner.ExecuteAsync(context);

        context.ExecutedSteps.ShouldBeEmpty();
    }
}
