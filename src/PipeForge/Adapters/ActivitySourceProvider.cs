#if !NETSTANDARD2_0
using System.Diagnostics;

namespace PipeForge.Adapters;

internal sealed class ActivitySourceProvider<T> : IPipelineDiagnostics<T>
{
    private static readonly ActivitySource _source = new($"PipeForge.PipelineRunner<{typeof(T).Name}>");
    private static readonly string _activityName = "PipelineStep";

    public IPipelineDiagnosticsScope BeginStep(IPipelineStep<T> step, int order)
    {
        var activity = _source.StartActivity(_activityName, ActivityKind.Internal);
        if (activity is not null)
        {
            activity.SetTag("pipeline.context_type", typeof(T).FullName);
            activity.SetTag("pipeline.step_name", step.Name);
            activity.SetTag("pipeline.step_order", order.ToString());
            activity.SetTag("pipeline.step_description", step.Description);
        }

        return new ActivityScope(activity);
    }

    public void ReportException(Exception ex, IPipelineStep<T> step, int order)
    {
        var activity = Activity.Current;
        if (activity is null) return;

        activity.SetTag("exception.type", ex.GetType().FullName);
        activity.SetTag("exception.message", ex.Message);
        activity.SetTag("exception.stacktrace", ex.ToString());
        activity.SetTag("otel.status_code", "ERROR");
        activity.SetTag("otel.status_description", ex.Message);
    }

    private sealed class ActivityScope : IPipelineDiagnosticsScope
    {
        private readonly Activity? _activity;

        public ActivityScope(Activity? activity)
        {
            _activity = activity;
        }

        public void Dispose()
        {
            _activity?.Dispose();
        }

        public void SetShortCircuited(bool value)
        {
            _activity?.SetTag("pipeline.short_circuited", value.ToString());
        }
    }
}
#endif
