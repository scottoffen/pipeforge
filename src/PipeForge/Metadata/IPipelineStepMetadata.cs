namespace PipeForge.Metadata;

/// <summary>
/// Provides descriptive metadata for a pipeline step used in diagnostics, documentation, or runtime introspection
/// </summary>
public interface IPipelineStepMetadata
{
    /// <summary>
    /// A brief description of the pipeline step for documentation or UI display
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// Indicates whether the pipeline step may terminate pipeline execution early without invoking subsequent steps
    /// </summary>
    bool MayShortCircuit { get; }

    /// <summary>
    /// A unique and human-readable name for the pipeline step
    /// </summary>
    string Name { get; }

    /// <summary>
    /// A descriptive explanation of the condition under which the pipeline step may short-circuit execution
    /// </summary>
    string? ShortCircuitCondition { get; }
}
