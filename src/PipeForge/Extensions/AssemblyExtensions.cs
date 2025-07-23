using System.Reflection;
using PipeForge.Metadata;

namespace PipeForge.Extensions;

/// <summary>
/// Extension methods for working with assemblies.
/// </summary>
internal static class AssemblyExtensions
{
    internal static readonly string MessageNotAnInterface = "The type '{0}' is not an interface.";

    /// <summary>
    /// Finds all types in the provided assemblies that are concrete, closed implementations of the specified interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IEnumerable<Type> FindClosedImplementationsOf<T>(this IEnumerable<Assembly> assemblies)
    {
        var targetInterface = typeof(T);
        if (!targetInterface.IsInterface)
        {
            var targetInterfaceName = targetInterface.GetTypeName();
            throw new ArgumentException(string.Format(MessageNotAnInterface, targetInterfaceName), nameof(T));
        }

        return assemblies
            .SelectMany(a => SafeGetTypes(() => a.GetTypes()))
            .Where(t =>
                targetInterface.IsAssignableFrom(t)
                 && t.IsClass
                 && !t.IsAbstract
                && !t.IsGenericTypeDefinition
                && !t.ContainsGenericParameters
            );
    }

    /// <summary>
    /// Retrieves pipeline step descriptors for the specified interface from
    /// the provided assemblies
    /// </summary>
    /// <remarks>
    /// Includes steps that match any of the (optional) provided filters.
    /// </remarks>
    /// <typeparam name="TStepInterface"></typeparam>
    /// <param name="assemblies"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    public static IEnumerable<PipelineStepDescriptor> GetDescriptorsFor<TStepInterface>(this IEnumerable<Assembly> assemblies, string[]? filters)
    {
        return assemblies
            .FindClosedImplementationsOf<TStepInterface>()
            .Select(t => new PipelineStepDescriptor(t))
            .Where(d => d.Filters.MatchesAnyFilter(filters))
            .OrderBy(d => d.Order);
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
