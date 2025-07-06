using Microsoft.Extensions.Logging;
using PipeForge.Tests.TestUtils;

namespace PipeForge.Tests.PipelineRunner;

public class LoggingTests
{
    private class TestStep : IPipelineStep<TestContext>
    {
        public string Name => "LogStep";
        public string Description => "Step that always runs";
        public bool MayShortCircuit => false;
        public string? ShortCircuitCondition => null;

        public Task InvokeAsync(TestContext context, PipelineDelegate<TestContext> next, CancellationToken cancellationToken = default)
            => next(context, cancellationToken);
    }

    private class ShortCircuitingStep : IPipelineStep<TestContext>
    {
        public string Name => "ShortCircuitingStep";
        public string Description => "Step that ends execution early";
        public bool MayShortCircuit => true;
        public string? ShortCircuitCondition => "Always";

        public Task InvokeAsync(TestContext context, PipelineDelegate<TestContext> next, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private static (PipelineRunner<TestContext> runner, TestLogger logger) CreateRunnerWithSteps(params IPipelineStep<TestContext>[] steps)
    {
        var provider = new TestLoggerProvider();
        var lazySteps = steps.Select(s => new Lazy<IPipelineStep<TestContext>>(() => s));
        var runner = new PipelineRunner<TestContext>(lazySteps, provider);
        return (runner, provider.Logger);
    }

    [Fact]
    public async Task Logs_Expected_Messages_During_Execution()
    {
        var (runner, logger) = CreateRunnerWithSteps(new TestStep());

        await runner.ExecuteAsync(new TestContext());

        logger.ShouldContainMessage("Executing pipeline step LogStep", LogLevel.Trace);
        logger.ShouldContainMessage("Completed pipeline step LogStep", LogLevel.Trace);
    }

    [Fact]
    public async Task Logs_ShortCircuit_Message_When_Step_Does_Not_Invoke_Next()
    {
        var (runner, logger) = CreateRunnerWithSteps(new ShortCircuitingStep());

        await runner.ExecuteAsync(new TestContext());

        logger.ShouldContainMessage("short-circuited", LogLevel.Information);
    }

    [Fact]
    public async Task Emits_Scope_For_Each_Step()
    {
        var (runner, logger) = CreateRunnerWithSteps(new TestStep());

        await runner.ExecuteAsync(new TestContext());

        logger.ShouldHaveScopeContaining("PipelineStepName", "LogStep");
        logger.ShouldHaveScopeContaining("PipelineContextType", nameof(TestContext));
        logger.ShouldHaveScopeContaining("PipelineStepOrder", 0);
    }
}
