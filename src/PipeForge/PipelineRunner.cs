using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PipeForge.Adapters.Diagnostics;
using PipeForge.Adapters.Json;

namespace PipeForge;

public abstract class PipelineRunner<TContext, TStepInterface> : IPipelineRunner<TContext, TStepInterface>
    where TContext : class
    where TStepInterface : IPipelineStep<TContext>
{
    private static readonly IJsonSerializer _jsonSerializer = JsonSerializerFactory.Create();
    private static readonly IPipelineDiagnostics<TContext> _diagnostics = PipelineDiagnosticsFactory.Create<TContext>();

    private static readonly string _pipelineContextType = "PipelineContextType";
    private static readonly string _pipelineStepName = "PipelineStepName";
    private static readonly string _pipelineStepOrder = "PipelineStepOrder";

    protected internal static readonly string MessagePipelineExecutionEnd = "Pipeline execution ended for {0}";
    protected internal static readonly string MessagePipelineExecutionStart = "Pipeline execution started for {0}";
    protected internal static readonly string MessageStepExecutionEnd = "Pipeline step completed {0} (Order {1}) in {2} ms";
    protected internal static readonly string MessageStepExecutionException = "Pipeline step exception {0} (Order {1})";
    protected internal static readonly string MessageStepExecutionStart = "Pipeline step executing {0} (Order {1})";
    protected internal static readonly string MessageStepShortCircuited = "Pipeline step short circuit {0} (Order {1}) after {2} ms";

    private readonly IServiceProvider _serviceProvider;

    protected readonly ILogger? Logger;
    protected IEnumerable<Lazy<TStepInterface>> LazySteps => _serviceProvider.GetServices<Lazy<TStepInterface>>();

    public PipelineRunner(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        Logger = _serviceProvider.GetService<ILoggerFactory>()?.CreateLogger(GetType());
    }

    public string Describe()
    {
        var steps = LazySteps
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
            ["title"] = "PipelineStep",
            ["type"] = "object",
            ["properties"] = new Dictionary<string, object?>
            {
                ["Order"] = new { type = "integer", description = "Execution order of the step (inferred)" },
                ["Name"] = new { type = "string", description = "Display name of the step" },
                ["Description"] = new { type = "string", description = "Optional description of the step" },
                ["MayShortCircuit"] = new { type = "boolean", description = "Whether the step may halt pipeline execution early" },
                ["ShortCircuitCondition"] = new { type = "string", description = "Explanation of the short-circuit condition, if any" },
            },
            ["required"] = new[] { "Order" }
        };

        return _jsonSerializer.Serialize(schema);
    }

    public async Task ExecuteAsync(TContext context, CancellationToken cancellationToken = default)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        PipelineDelegate<TContext> next = (_, _) => Task.CompletedTask;

        var steps = LazySteps
            .Select((lazy, index) => (LazyStep: lazy, Order: index))
            .Reverse()
            .ToList();

        foreach (var (lazyStep, order) in steps)
        {
            var previous = next;

            next = async (ctx, ct) =>
            {
                var step = lazyStep.Value;

                using (Logger?.BeginScope(new Dictionary<string, object>
                {
                    [_pipelineContextType] = typeof(TContext).Name,
                    [_pipelineStepName] = step.Name,
                    [_pipelineStepOrder] = order
                }))
                {
                    using var activity = _diagnostics.BeginStep(step, order);

                    try
                    {
                        if (ct.IsCancellationRequested)
                        {
                            Logger?.LogInformation("Pipeline cancelled before executing step {Step} (Order {Order})", step.Name, order);
                            activity?.SetCanceled();
                            ct.ThrowIfCancellationRequested();
                        }

                        var sw = Stopwatch.StartNew();
                        var wasCalled = false;

                        async Task wrappedNext(TContext c, CancellationToken token = default)
                        {
                            wasCalled = true;
                            await previous(c, token);
                        }

                        Logger?.LogTrace(MessageStepExecutionStart, step.Name, order);
                        await step.InvokeAsync(ctx, wrappedNext, ct);
                        sw.Stop();

                        if (wasCalled)
                        {
                            Logger?.LogTrace(MessageStepExecutionEnd, step.Name, order, sw.ElapsedMilliseconds);
                        }
                        else
                        {
                            Logger?.LogInformation(MessageStepShortCircuited, step.Name, order, sw.ElapsedMilliseconds);
                        }

                        activity?.SetShortCircuited(!wasCalled);
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException and not PipelineExecutionException<TContext>)
                    {
                        _diagnostics.ReportException(ex, step, order);
                        Logger?.LogError(ex, MessageStepExecutionException, step.Name, order);

                        throw new PipelineExecutionException<TContext>(step.Name, order, ex);
                    }
                }
            };
        }

        Logger?.LogDebug(MessagePipelineExecutionStart, GetType().Name);
        await next(context, cancellationToken);
        Logger?.LogDebug(MessagePipelineExecutionEnd, GetType().Name);
    }
}

[ExcludeFromCodeCoverage]
public class PipelineRunner<TContext> : PipelineRunner<TContext, IPipelineStep<TContext>>, IPipelineRunner<TContext>
    where TContext : class
{
    public PipelineRunner(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }
}
