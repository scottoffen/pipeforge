namespace PipeForge.Extensions;

public static class TypeExtensions
{
    private static readonly Type _pipelineStepType = typeof(IPipelineStep<>);

    /// <summary>
    /// Checks if the type implements the IPipelineStep interface.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool ImplementsPipelineStep(this Type type)
    {
        if (type == null) return false;
        return (type.IsGenericType && type.GetGenericTypeDefinition() == _pipelineStepType) ||
            type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == _pipelineStepType);
    }

    /// <summary>
    /// Checks if the type directly implements the specified interface type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="interfaceType"></param>
    /// <returns></returns>
    public static bool DirectlyImplements(this Type type, Type interfaceType)
    {
        var baseInterfaces = type.BaseType?.GetInterfaces() ?? [];
        var directInterfaces = type.GetInterfaces().Except(baseInterfaces);
        return directInterfaces.Contains(interfaceType);
    }
}
