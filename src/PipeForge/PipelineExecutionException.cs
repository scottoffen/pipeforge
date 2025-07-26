using System.Diagnostics;

namespace PipeForge;

/// <summary>
/// Represents an exception thrown during the execution of a pipeline step for the specified context type
/// </summary>
/// <typeparam name="TContext">The type of context used in the pipeline</typeparam>
[DebuggerDisplay("StepName = {StepName}, StepOrder = {StepOrder}, Message = {Message}")]
public class PipelineExecutionException<TContext> : Exception
{
    /// <summary>
    /// The name of the step where the exception occurred
    /// </summary>
    public string StepName { get; }

    /// <summary>
    /// The execution order of the step where the exception occurred
    /// </summary>
    public int StepOrder { get; }

    /// <summary>
    /// Initializes a new instance of the exception using the step name and order, with an optional inner exception
    /// </summary>
    /// <param name="stepName">The name of the step that caused the exception</param>
    /// <param name="stepOrder">The order of the step in the pipeline</param>
    /// <param name="innerException">The inner exception that caused the failure, if any</param>
    public PipelineExecutionException(string stepName, int stepOrder, Exception? innerException = null)
        : this(stepName, stepOrder, $"Exception in step '{stepName}' (Order: {stepOrder}).", innerException)
    {

    }

    /// <summary>
    /// Initializes a new instance of the exception using the step name, order, custom message, and optional inner exception
    /// </summary>
    /// <param name="stepName">The name of the step that caused the exception</param>
    /// <param name="stepOrder">The order of the step in the pipeline</param>
    /// <param name="message">A custom message describing the exception</param>
    /// <param name="innerException">The inner exception that caused the failure, if any</param>
    public PipelineExecutionException(string stepName, int stepOrder, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        StepName = stepName;
        StepOrder = stepOrder;

        Data["PipelineStepName"] = stepName;
        Data["PipelineStepOrder"] = stepOrder;
        Data["PipelineContextType"] = typeof(TContext).FullName;
    }
}
