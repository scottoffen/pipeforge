using PipeForge.Tests.TestUtils;

namespace PipeForge.Tests;

public class PipelineBuilderTests
{
    [Fact]
    public async Task PipelineBuilder_CreatesAndRunsPipeline_WithoutLoggerFactory()
    {
        var pipeline = Pipeline.CreateFor<TestContext>()
            .WithStep<StepA>()
            .WithStep<StepB>()
            .WithStep<StepC>()
            .Build();

        var context = new TestContext();
        await pipeline.ExecuteAsync(context);

        string.Join("", context.ExecutedSteps).ShouldBe("ABC");
    }

    [Fact]
    public async Task PipelineBuilder_CreatesAndRunsPipeline_WithLoggerFactory()
    {
        var loggerFactory = new TestLoggerProvider();
        var pipeline = Pipeline.CreateFor<TestContext>(loggerFactory)
            .WithStep<StepA>()
            .WithStep<StepB>()
            .WithStep<StepC>()
            .Build();

        var context = new TestContext();
        await pipeline.ExecuteAsync(context);

        string.Join("", context.ExecutedSteps).ShouldBe("ABC");

        var logger = loggerFactory.CreateLogger("something");
        logger.ShouldNotBeNull();
        var testLogger = logger as TestLogger;
        testLogger.ShouldNotBeNull();

        testLogger.LogEntries.Count.ShouldBe(6);
    }

    [Fact]
    public async Task PipelineBuilder_CreatesAndRunsPipeline_UsingStepFactory()
    {
        var expected = Guid.NewGuid().ToString();
        var pipeline = Pipeline.CreateFor<TestContext>()
            .WithStep<StepA>()
            .WithStep<StepB>()
            .WithStep<StepC>()
            .WithStep(() => new StepD(expected))
            .Build();

        var context = new TestContext();
        await pipeline.ExecuteAsync(context);

        string.Join("", context.ExecutedSteps).ShouldBe($"ABC{expected}");
    }

    private abstract class TestStep : PipelineStep<TestContext>
    {
        public override async Task InvokeAsync(TestContext context, PipelineDelegate<TestContext> next, CancellationToken cancellationToken = default)
        {
            context.ExecutedSteps.Add(Name);
            await next(context, cancellationToken);
        }
    }

    private class StepA : TestStep
    {
        public StepA() => Name = "A";
    }

    private class StepB : TestStep
    {
        public StepB() => Name = "B";
    }

    private class StepC : TestStep
    {
        public StepC() => Name = "C";
    }

    private class StepD : TestStep
    {
        public StepD(string name) => Name = name;
    }
}
