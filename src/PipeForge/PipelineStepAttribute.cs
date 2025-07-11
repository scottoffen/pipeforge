namespace PipeForge;

/// <summary>
/// Marks a class as a pipeline step and provides metadata for ordering and filter targeting
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PipelineStepAttribute : Attribute
{
    /// <summary>
    /// The order in which the step should be executed relative to other steps
    /// </summary>
    /// <remarks>
    /// Lower numbers are executed first. Steps with the same order are executed in the order they are registered.
    /// </remarks>
    public int Order { get; }

    /// <summary>
    /// An optional step filter, such as "Production" or "Development"
    /// </summary>
    public string? Filter { get; }

    /// <summary>
    /// Initializes a new instance of the attribute with the specified order, enabled state, and filter.
    /// </summary>
    /// <param name="order">The execution order of the step</param>
    /// <param name="filter">The filter in which the step should be active</param>
    public PipelineStepAttribute(int order, string? filter = null)
    {
        Order = order;
        Filter = filter;
    }
}
