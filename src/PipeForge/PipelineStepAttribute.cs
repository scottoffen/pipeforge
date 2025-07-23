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
    /// Lower numbers are executed first. Steps with the same value for order are executed in the order they are registered (or discovered).
    /// </remarks>
    public int Order { get; }

    /// <summary>
    /// An optional step filter, such as "Production" or "Development"
    /// </summary>
    /// <remarks>
    /// Supports multiple filters, allowing the step to be active in different contexts.
    /// </remarks>
    public IEnumerable<string> Filters { get; }

    #region CLS Compliant Constructors

    /// <summary>
    /// Initializes a new instance of the attribute with the specified order.
    /// </summary>
    /// <param name="order">The execution order of the step</param>
    public PipelineStepAttribute(int order)
    {
        Order = order;
        Filters = [];
    }

    /// <summary>
    /// Initializes a new instance of the attribute with the specified order and single filter.
    /// </summary>
    /// <param name="order">The execution order of the step</param>
    /// <param name="filter">The filter in which the step should be active</param>
    public PipelineStepAttribute(int order, string filter)
    {
        Order = order;
        Filters = string.IsNullOrWhiteSpace(filter)
            ? []
            : [filter];
    }

    #endregion

    /// <summary>
    /// Initializes a new instance of the attribute with the specified order and filters.
    /// </summary>
    /// <param name="order">The execution order of the step</param>
    /// <param name="filters">The filters in which the step should be active</param>
    public PipelineStepAttribute(int order, params string[] filters)
    {
        Order = order;
        Filters = filters?.Where(f => !string.IsNullOrWhiteSpace(f)) ?? [];
    }
}
