namespace PipeForge;

/// <summary>
/// Marks a class as a pipeline step and provides metadata for ordering, activation, and environment targeting
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
    /// Indicates whether the step is enabled by default
    /// </summary>
    public bool IsEnabled { get; }

    /// <summary>
    /// The environment in which the step should be included, such as "Production" or "Development"
    /// </summary>
    public string? Environment { get; }

    /// <summary>
    /// Initializes a new instance of the attribute with the specified order and default enabled state.
    /// </summary>
    /// <param name="order"></param>
    public PipelineStepAttribute(int order) : this(order, true, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the attribute with the specified order and enabled state.
    /// </summary>
    /// <param name="order"></param>
    /// <param name="isEnabled"></param>
    public PipelineStepAttribute(int order, bool isEnabled) : this(order, isEnabled, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the attribute with the specified order and environment.
    /// </summary>
    /// <param name="order"></param>
    /// <param name="environment"></param>
    public PipelineStepAttribute(int order, string environment) : this(order, true, environment)
    {
    }

    /// <summary>
    /// Initializes a new instance of the attribute with the specified order, enabled state, and environment.
    /// </summary>
    /// <param name="order">The execution order of the step</param>
    /// <param name="isEnabled">Whether the step is enabled by default</param>
    /// <param name="environment">The environment in which the step should be active</param>
    public PipelineStepAttribute(int order, bool isEnabled, string? environment = null)
    {
        Order = order;
        IsEnabled = isEnabled;
        Environment = environment;
    }
}
