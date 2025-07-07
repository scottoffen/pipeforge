using System.Reflection;

namespace PipeForge.Metadata;

/// <summary>
/// Represents metadata extracted from a pipeline step implementation and its associated attribute
/// </summary>
public sealed class PipelineStepDescriptor
{
    /// <summary>
    /// The concrete type that implements the pipeline step
    /// </summary>
    public Type ImplementationType { get; }

    /// <summary>
    /// The order in which the step should be executed relative to other steps
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// The environment in which the step should be included, such as "Production" or "Development"
    /// </summary>
    public string? Environment { get; }

    /// <summary>
    /// Creates a descriptor from the pipeline step type and extracts its metadata from the PipelineStepAttribute
    /// </summary>
    /// <param name="implementationType">The type that implements the pipeline step</param>
    /// <exception cref="InvalidOperationException">Thrown if the type does not have a PipelineStepAttribute</exception>
    public PipelineStepDescriptor(Type implementationType)
    {
        ImplementationType = implementationType;

        var attribute = implementationType.GetCustomAttribute<PipelineStepAttribute>()
            ?? throw new InvalidOperationException($"Pipeline step '{implementationType.FullName}' must be decorated with [PipelineStep].");

        Order = attribute.Order;
        Environment = attribute.Environment;
    }
}
