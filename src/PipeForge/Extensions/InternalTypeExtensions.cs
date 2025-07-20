namespace PipeForge.Extensions;

/// <summary>
/// Extension methods for Type to check if it implements IPipelineStep.
/// and if it is a closed generic type.
/// This is used to determine if a type can be used as a pipeline step in PipeForge.
/// </summary>
internal static class InternalTypeExtensions
{
    private static readonly Type _pipelineStepType = typeof(IPipelineStep<>);

    /// <summary>
    /// Checks if the type is a class, not abstract, and implements IPipelineStep.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsPipelineStep<T>(this Type type)
    {
        var targetInterface = typeof(T);

        return type.IsClass &&
            !type.IsAbstract &&
            !type.IsGenericTypeDefinition &&
            !type.ContainsGenericParameters &&
            targetInterface.IsAssignableFrom(type) &&
            type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == _pipelineStepType);
    }

    /// <summary>
    /// Checks if the type implements the IPipelineStep interface.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool ImplementsPipelineStep(this Type type)
    {
        if (type == null) return false;
        return type.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == _pipelineStepType)
            || (type.IsGenericType && type.GetGenericTypeDefinition() == _pipelineStepType);
    }
}
