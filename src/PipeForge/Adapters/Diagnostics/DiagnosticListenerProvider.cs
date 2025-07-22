#if NETSTANDARD2_0
using System.Diagnostics;

namespace PipeForge.Adapters.Diagnostics;

internal sealed class DiagnosticListenerProvider<T> : IPipelineDiagnostics<T>
{
    private static readonly DiagnosticListener _listener = new($"PipeForge.PipelineRunner<{typeof(T).Name}>");
    private static readonly string _activityName = "PipelineStep";

    public IPipelineDiagnosticsScope BeginStep(IPipelineStep<T> step, int order)
    {
        if (!_listener.IsEnabled() || !_listener.IsEnabled(_activityName))
            return NullScope.Instance;

        var activity = new Activity(_activityName);
        var metadata = new ActivityMetadata
        {
            context_type = typeof(T).FullName,
            step_name = step.Name,
            step_order = order,
            step_description = step.Description
        };

        _listener.StartActivity(activity, metadata);

        return new DiagnosticListenerScope(_listener, activity, metadata);
    }

    public void ReportException(Exception ex, IPipelineStep<T> step, int order)
    {
        _listener.Write("PipelineStep.Exception", new
        {
            context_type = typeof(T).FullName,
            step_name = step.Name,
            step_order = order,
            exception_type = ex.GetType().FullName,
            exception_message = ex.Message,
            exception_stacktrace = ex.ToString()
        });
    }

    private sealed class DiagnosticListenerScope : IPipelineDiagnosticsScope
    {
        private readonly DiagnosticListener _listener;
        private readonly Activity _activity;
        private readonly ActivityMetadata _metadata;

        public DiagnosticListenerScope(DiagnosticListener listener, Activity activity, ActivityMetadata metadata)
        {
            _listener = listener;
            _activity = activity;
            _metadata = metadata;
        }

        public void Dispose() => _listener.StopActivity(_activity, _metadata);

        public void SetShortCircuited(bool value)
        {
            _metadata.short_circuited = value;
        }
    }

    private sealed class ActivityMetadata
    {
        public string context_type { get; set; } = null!;
        public string step_name { get; set; } = null!;
        public int step_order { get; set; }
        public string? step_description { get; set; }
        public bool short_circuited { get; set; }

        public override string ToString()
        {
            return $"{{ context_type = {context_type}, step_name = {step_name}, step_order = {step_order}, step_description = {step_description}, short_circuited = {short_circuited} }}";
        }
    }
}
#endif
