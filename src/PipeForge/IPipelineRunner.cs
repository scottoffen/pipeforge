namespace PipeForge;

/// <summary>
/// Represents a pipeline runner for a specific context type, using the provided interface as the step interface.
/// </summary>
/// <typeparam name="TContext">
/// The type of the context that is passed to each pipeline step.
/// </typeparam>
/// <typeparam name="TStepInterface">
/// The interface that all pipeline steps implement. Must be assignable to <see cref="IPipelineStep{TContext}"/>.
/// </typeparam>
public interface IPipelineRunner<TContext, TStepInterface>
    where TContext : class
    where TStepInterface : IPipelineStep<TContext>
{
    /// <summary>
    /// Executes all registered pipeline steps in order using the provided context.
    /// </summary>
    /// <param name="context">The context instance to pass to each step.</param>
    /// <param name="cancellationToken">A token to cancel the execution pipeline.</param>
    Task ExecuteAsync(TContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a JSON array describing the steps in the pipeline
    /// </summary>
    string Describe();

    /// <summary>
    /// Returns the JSON schema representing the structure of the pipeline step metadata
    /// </summary>
    string DescribeSchema();
}

/// <summary>
/// Represents a pipeline runner for a specific context type, using <see cref="IPipelineStep{TContext}"/> as the step interface.
/// </summary>
/// <typeparam name="TContext">
/// The type of the context that is passed to each pipeline step.
/// </typeparam>
/// <remarks>
/// This interface simplifies registration and resolution when a custom step interface is not needed.
/// </remarks>
public interface IPipelineRunner<TContext> : IPipelineRunner<TContext, IPipelineStep<TContext>>
    where TContext : class
{ }


