using System.Diagnostics;
using PipeForge.Adapters;

using Microsoft.Extensions.Logging;

namespace PipeForge;

/// <summary>
/// Executes a composed pipeline of steps with optional structured logging and tracing.
/// </summary>
/// <typeparam name="T">The pipeline context type</typeparam>
public class PipelineRunner<T> : IPipelineRunner<T>
{
    private static readonly string ActivityName = "PipelineStep";
    private static readonly IJsonSerializer _jsonSerializer = JsonSerializerFactory.Create();

    private readonly IEnumerable<Lazy<IPipelineStep<T>>> _lazySteps;
    private readonly ILogger? _logger;

#if NETSTANDARD2_0
    private static readonly DiagnosticListener DiagnosticListener = new($"PipeForge.PipelineRunner<{typeof(T).Name}>");
#else
    private static readonly ActivitySource ActivitySource = new($"PipeForge.PipelineRunner<{typeof(T).Name}>");
#endif

    public PipelineRunner(IEnumerable<Lazy<IPipelineStep<T>>> lazySteps, ILoggerFactory? loggerFactory = null)
    {
        _lazySteps = lazySteps;
        _logger = loggerFactory?.CreateLogger($"PipelineRunner<{typeof(T).Name}>");
    }

    public string Describe()
    {
        var steps = _lazySteps
            .Select(s => s.Value)
            .Select((step, index) => new
            {
                Order = index,
                step.Name,
                step.Description,
                step.MayShortCircuit,
                step.ShortCircuitCondition
            });

        return _jsonSerializer.Serialize(steps);
    }

    public string DescribeSchema()
    {
        var schema = new Dictionary<string, object?>
        {
            ["$schema"] = "http://json-schema.org/draft-07/schema#",
            ["title"] = ActivityName,
            ["type"] = "object",
            ["properties"] = new Dictionary<string, object?>
            {
                ["Order"] = new { type = "integer", description = "Execution order of the step (inferred)" },
                ["Name"] = new { type = "string", description = "Display name of the step" },
                ["Description"] = new { type = "string", description = "Optional description of the step" },
                ["MayShortCircuit"] = new { type = "boolean", description = "Whether the step may halt pipeline execution early" },
                ["ShortCircuitCondition"] = new { type = "string", description = "Explanation of the short-circuit condition, if any" },
            },
            ["required"] = new[] { "Order", "Name", "MayShortCircuit" }
        };

        return _jsonSerializer.Serialize(schema);
    }

    public async Task ExecuteAsync(T context, CancellationToken cancellationToken = default)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        PipelineDelegate<T> next = (_, _) => Task.CompletedTask;

        var steps = _lazySteps
            .Select((lazy, index) => (LazyStep: lazy, Order: index))
            .Reverse()
            .ToList();

        foreach (var (lazyStep, order) in steps)
        {
            var previous = next;

            next = async (ctx, ct) =>
            {
                var step = lazyStep.Value;

                using (_logger?.BeginScope(new Dictionary<string, object>
                {
                    ["PipelineContextType"] = typeof(T).Name,
                    ["PipelineStepName"] = step.Name,
                    ["PipelineStepOrder"] = order
                }))
                {
#if NETSTANDARD2_0
                    Activity? activity = null;
                    if (DiagnosticListener.IsEnabled() && DiagnosticListener.IsEnabled(ActivityName))
                    {
                        activity = new Activity(ActivityName);
                        DiagnosticListener.StartActivity(activity, GetStepMetadata(step, order, wasCalled: null));
                    }
#else
                    using var activity = ActivitySource.StartActivity(ActivityName, ActivityKind.Internal);
                    EnrichActivity(activity, step, order);
#endif
                    try
                    {
                        var sw = Stopwatch.StartNew();
                        var wasCalled = false;

                        async Task wrappedNext(T c, CancellationToken token = default)
                        {
                            wasCalled = true;
                            await previous(c, token);
                        }

                        _logger?.LogTrace("Executing pipeline step {StepName} (Order {StepOrder})", step.Name, order);
                        await step.InvokeAsync(ctx, wrappedNext, ct);
                        sw.Stop();

                        if (wasCalled)
                        {
                            _logger?.LogTrace("Completed pipeline step {StepName} (Order {StepOrder}) in {Elapsed} ms",
                                step.Name, order, sw.ElapsedMilliseconds);
                        }
                        else
                        {
                            _logger?.LogInformation("Pipeline short-circuited by step {StepName} (Order {StepOrder}) after {Elapsed} ms",
                                step.Name, order, sw.ElapsedMilliseconds);
                        }

#if NETSTANDARD2_0
                        if (activity != null)
                        {
                            DiagnosticListener.StopActivity(activity, GetStepMetadata(step, order, wasCalled));
                        }
#else
                        activity?.SetTag("pipeline.short_circuited", (!wasCalled).ToString());
#endif
                    }
                    catch (Exception ex)
                    {
#if NETSTANDARD2_0
                        if (activity != null)
                        {
                            DiagnosticListener.Write("PipelineStep.Exception", new
                            {
                                context_type = typeof(T).FullName,
                                step_name = step.Name,
                                step_order = order,
                                exception_type = ex.GetType().FullName,
                                exception_message = ex.Message,
                                exception_stacktrace = ex.ToString()
                            });
                        }
#else
                        activity?.SetTag("exception.type", ex.GetType().FullName);
                        activity?.SetTag("exception.message", ex.Message);
                        activity?.SetTag("exception.stacktrace", ex.ToString());
                        activity?.SetTag("otel.status_code", "ERROR");
                        activity?.SetTag("otel.status_description", ex.Message);
#endif
                        _logger?.LogError(ex, "Exception in pipeline step {StepName} (Order {StepOrder})", step.Name, order);

                        if (ex is PipelineExecutionException<T>) throw;

                        throw new PipelineExecutionException<T>(step.Name, order, ex);
                    }
                }
            };
        }

        await next(context, cancellationToken);
    }

#if NETSTANDARD2_0
    /// <summary>
    /// Creates an anonymous object for DiagnosticListener metadata.
    /// </summary>
    private static object GetStepMetadata(IPipelineStep<T> step, int order, bool? wasCalled)
    {
        return new
        {
            context_type = typeof(T).FullName,
            step_name = step.Name,
            step_order = order,
            step_description = step.Description,
            short_circuited = wasCalled.HasValue ? !wasCalled.Value : (bool?)null
        };
}
#else
    /// <summary>
    /// Adds structured tags to an Activity for OpenTelemetry.
    /// </summary>
    private static void EnrichActivity(Activity? activity, IPipelineStep<T> step, int order)
    {
        if (activity is null) return;

        activity.SetTag("pipeline.context_type", typeof(T).FullName);
        activity.SetTag("pipeline.step_name", step.Name);
        activity.SetTag("pipeline.step_order", order.ToString());
        activity.SetTag("pipeline.step_description", step.Description);
    }
#endif
}
