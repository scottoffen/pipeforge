namespace PipeForge;

/// <summary>
/// A pipeline step that delegates execution to a provided function.
/// </summary>
/// <typeparam name="TContext">The type of the pipeline context.</typeparam>
internal sealed class DelegatePipelineStep<TContext> : PipelineStep<TContext>
    where TContext : class
{
    private readonly Func<TContext, PipelineDelegate<TContext>, CancellationToken, Task> _invoke;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegatePipelineStep{TContext}"/> class with a delegate to execute.
    /// </summary>
    /// <param name="invoke">The delegate to invoke during pipeline execution.</param>
    public DelegatePipelineStep(Func<TContext, PipelineDelegate<TContext>, CancellationToken, Task> invoke)
    {
        _invoke = invoke ?? throw new ArgumentNullException(nameof(invoke));
    }

    public override Task InvokeAsync(TContext context, PipelineDelegate<TContext> next, CancellationToken cancellationToken = default)
    {
        return _invoke(context, next, cancellationToken);
    }
}
