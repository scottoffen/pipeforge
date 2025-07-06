using System.Diagnostics;
using PipeForge.Tests.TestUtils;
using Microsoft.Extensions.Logging.Abstractions;

namespace PipeForge.Tests.PipelineRunner;

[Collection("DiagnosticTests")]
public class DiagnosticTests
{
    private class TestStep : IPipelineStep<TestContext>
    {
        public string Name => "TestStep";
        public string Description => "A step for testing";
        public bool MayShortCircuit => false;
        public string? ShortCircuitCondition => null;

        public Task InvokeAsync(TestContext context, PipelineDelegate<TestContext> next, CancellationToken cancellationToken = default)
        {
            return next(context, cancellationToken);
        }
    }

    private class ShortCircuitingStep : IPipelineStep<TestContext>
    {
        public string Name => "ShortCircuitingStep";
        public string Description => "Stops the pipeline";
        public bool MayShortCircuit => true;
        public string? ShortCircuitCondition => "Always stops";

        public Task InvokeAsync(TestContext context, PipelineDelegate<TestContext> next, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private class ExceptionStep : IPipelineStep<TestContext>
    {
        public string Name => "ExceptionStep";
        public string Description => "Throws an error";
        public bool MayShortCircuit => false;
        public string? ShortCircuitCondition => null;

        public Task InvokeAsync(TestContext context, PipelineDelegate<TestContext> next, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Boom");
        }
    }

    [Fact]
    public async Task StartsAndTagsActivityCorrectly()
    {
        var activities = new List<Activity>();
        var expectedSource = $"PipeForge.PipelineRunner<{typeof(TestContext).Name}>";

        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == expectedSource,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStarted = activity => activities.Add(activity),
            ActivityStopped = _ => { }
        };

        ActivitySource.AddActivityListener(listener);

        var runner = new PipelineRunner<TestContext>(
            new[] { new Lazy<IPipelineStep<TestContext>>(() => new TestStep()) },
            NullLoggerFactory.Instance);

        await runner.ExecuteAsync(new TestContext());

        var activity = activities.SingleOrDefault(a => a.DisplayName == "PipelineStep");
        activity.ShouldNotBeNull();
        activity.Tags.ShouldContain(t => t.Key == "pipeline.step_name" && t.Value == "TestStep");
        activity.Tags.ShouldContain(t => t.Key == "pipeline.short_circuited" && t.Value == "False");
    }

    [Fact]
    public async Task RecordsShortCircuitInActivityTag()
    {
        var activities = new List<Activity>();
        var expectedSource = $"PipeForge.PipelineRunner<{typeof(TestContext).Name}>";

        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == expectedSource,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStarted = activity => activities.Add(activity),
            ActivityStopped = _ => { }
        };

        ActivitySource.AddActivityListener(listener);

        var runner = new PipelineRunner<TestContext>(
            new[] { new Lazy<IPipelineStep<TestContext>>(() => new ShortCircuitingStep()) },
            NullLoggerFactory.Instance);

        await runner.ExecuteAsync(new TestContext());

        var activity = activities.SingleOrDefault(a => a.DisplayName == "PipelineStep");
        activity.ShouldNotBeNull();
        activity.Tags.ShouldContain(t => t.Key == "pipeline.step_name" && t.Value == "ShortCircuitingStep");
        activity.Tags.ShouldContain(t => t.Key == "pipeline.short_circuited" && t.Value == "True");
    }

    [Fact]
    public async Task RecordsExceptionDetailsInActivityTags()
    {
        var activities = new List<Activity>();
        var expectedSource = $"PipeForge.PipelineRunner<{typeof(TestContext).Name}>";

        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == expectedSource,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStarted = activity => activities.Add(activity),
            ActivityStopped = _ => { }
        };

        ActivitySource.AddActivityListener(listener);

        var runner = new PipelineRunner<TestContext>(
            new[] { new Lazy<IPipelineStep<TestContext>>(() => new ExceptionStep()) },
            NullLoggerFactory.Instance);

        var ex = await Should.ThrowAsync<PipelineExecutionException<TestContext>>(async () =>
        {
            await runner.ExecuteAsync(new TestContext());
        });

        ex.StepName.ShouldBe("ExceptionStep");
        ex.StepOrder.ShouldBe(0);

        var activity = activities.SingleOrDefault(a => a.DisplayName == "PipelineStep");
        activity.ShouldNotBeNull();
        activity.Tags.ShouldContain(t => t.Key == "exception.type" && t.Value == typeof(InvalidOperationException).FullName);
        activity.Tags.ShouldContain(t => t.Key == "otel.status_code" && t.Value == "ERROR");
    }
}
