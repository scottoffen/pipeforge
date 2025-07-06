using PipeForge.Metadata;

namespace PipeForge;

/// <summary>
/// Represents the next step to be invoked in the pipeline sequence.
/// </summary>
public delegate Task PipelineDelegate<T>(T context, CancellationToken cancellationToken = default);

/// <summary>
/// Defines a marker interface for all pipeline steps.
/// </summary>
/// <remarks>
/// Do not implement this interface directly.
/// Instead, implement <c>IPipelineStep&lt;T&gt;</c> for steps that operate on a specific context type.
/// </remarks>
public interface IPipelineStep { }

/// <summary>
/// Defines a single executable step in the pipeline that operates on a context of type <typeparamref name="T"/> 
/// and may alter the context and/or short-circuit the pipeline
/// </summary>
/// <remarks>
/// Instead of implementing this interface directly, you should consider extending the abstract class <c>PipelineStep&lt;T&gt;</c>, and setting the properties of the step in the constructor.
/// </remarks>
/// <typeparam name="T">The type of context passed through the pipeline</typeparam>
public interface IPipelineStep<T> : IPipelineStep, IPipelineStepMetadata
{
    /// <summary>
    /// Executes the logic for this step and optionally invokes the next step in the pipeline
    /// </summary>
    /// <param name="context">The context object to process</param>
    /// <param name="next">A delegate representing the next step in the pipeline</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation</param>
    Task InvokeAsync(T context, PipelineDelegate<T> next, CancellationToken cancellationToken = default);
}
