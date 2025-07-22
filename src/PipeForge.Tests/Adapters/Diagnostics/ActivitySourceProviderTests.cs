using System.Diagnostics;
using PipeForge.Adapters.Diagnostics;
using PipeForge.Tests.Steps;

namespace PipeForge.Tests.Adapters.Diagnostics;

public class ActivitySourceProviderTests
{
    private static ActivityListener SetupActivityListener(string sourceName, List<Activity> activities)
    {
        var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == sourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStarted = activity => activities.Add(activity),
            ActivityStopped = _ => { }
        };

        ActivitySource.AddActivityListener(listener);
        return listener;
    }

    [Fact]
    public void BeginStep_StartsActivity_WithExpectedTags()
    {
        var activities = new List<Activity>();
        var expectedSource = $"PipeForge.PipelineRunner<{typeof(SampleContext).Name}>";

        using var _ = SetupActivityListener(expectedSource, activities);

        var provider = new ActivitySourceProvider<SampleContext>();
        var step = new SampleContextStepA();
        using var scope = provider.BeginStep(step, 1);

        activities.ShouldHaveSingleItem();
        var activity = activities[0];

        activity.DisplayName.ShouldBe("PipelineStep");
        activity.GetTagItem("pipeline.context_type").ShouldBe(typeof(SampleContext).FullName);
        activity.GetTagItem("pipeline.step_name").ShouldBe(step.Name);
        activity.GetTagItem("pipeline.step_order").ShouldBe("1");
        activity.GetTagItem("pipeline.step_description").ShouldBe(step.Description);
    }

    [Fact]
    public void ReportException_SetsExpectedTags_OnCurrentActivity()
    {
        var activities = new List<Activity>();
        var expectedSource = $"PipeForge.PipelineRunner<{typeof(SampleContext).Name}>";

        using var _ = SetupActivityListener(expectedSource, activities);

        var provider = new ActivitySourceProvider<SampleContext>();
        var step = new SampleContextStepA();
        using var scope = provider.BeginStep(step, 2);
        var ex = new InvalidOperationException("oops");

        provider.ReportException(ex, step, 2);

        var activity = activities[0];
        activity.GetTagItem("exception.type").ShouldBe(typeof(InvalidOperationException).FullName);
        activity.GetTagItem("exception.message").ShouldBe("oops");
        activity.GetTagItem("otel.status_code").ShouldBe("ERROR");
        activity.GetTagItem("otel.status_description").ShouldBe("oops");

        var stacktrace = activity.GetTagItem("exception.stacktrace") as string;
        stacktrace.ShouldNotBeNull();
        stacktrace.ShouldContain("InvalidOperationException");
    }

    [Fact]
    public void Scope_SetCanceled_SetsTag()
    {
        var activities = new List<Activity>();
        var expectedSource = $"PipeForge.PipelineRunner<{typeof(SampleContext).Name}>";

        using var _ = SetupActivityListener(expectedSource, activities);

        var provider = new ActivitySourceProvider<SampleContext>();
        var step = new SampleContextStepA();
        using var scope = provider.BeginStep(step, 3);

        scope.SetCanceled();

        activities[0].GetTagItem("pipeline.cancelled").ShouldBe(true);
    }

    [Fact]
    public void Scope_SetShortCircuited_SetsTag()
    {
        var activities = new List<Activity>();
        var expectedSource = $"PipeForge.PipelineRunner<{typeof(SampleContext).Name}>";

        using var _ = SetupActivityListener(expectedSource, activities);

        var provider = new ActivitySourceProvider<SampleContext>();
        var step = new SampleContextStepA();
        using var scope = provider.BeginStep(step, 4);

        scope.SetShortCircuited(true);

        activities[0].GetTagItem("pipeline.short_circuited").ShouldBe("True");
    }
}
