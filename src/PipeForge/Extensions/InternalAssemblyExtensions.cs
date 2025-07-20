using System.Reflection;

namespace PipeForge.Extensions;

/// <summary>
/// Extension methods for working with assemblies.
/// </summary>
internal static class InternalAssemblyExtensions
{
    /// <summary>
    /// Finds all types in the provided assemblies that implement the specified interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IEnumerable<Type> FindImplementationsOf<T>(this IEnumerable<Assembly> assemblies)
    {
        return assemblies
            .SelectMany(a => SafeGetTypes(() => a.GetTypes()))
            .Where(t => t.IsPipelineStep<T>());
    }

    /// <summary>
    /// <para>Safely retrieves types from an assembly, handling exceptions that may occur during reflection.</para>
    /// Returns:
    /// <list type="bullet">
    /// <item>A collection of types from the provided delegate.</item>
    /// <item>If a ReflectionTypeLoadException is thrown, it filters out null types.</item>
    /// <item>For any other exception, it returns an empty collection.</item>
    /// </list>
    /// </summary>
    /// <param name="getTypes"></param>
    internal static IEnumerable<Type> SafeGetTypes(Func<Type[]> getTypes)
    {
        try
        {
            return getTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t is not null)!;
        }
        catch
        {
            return [];
        }
    }
}
