using System.Reflection;

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
            var targetInterfaceName = targetInterface.FullName ?? targetInterface.Name;
            throw new ArgumentException(string.Format(MessageNotAnInterface, targetInterfaceName), nameof(T));
        }

        return assemblies
            .SelectMany(a => SafeGetTypes(() => a.GetTypes()))
            .Where(t =>
                targetInterface.IsAssignableFrom(t) &&
                t.IsClass &&
                !t.IsInterface &&
                !t.IsAbstract &&
                !t.IsGenericTypeDefinition &&
                !t.ContainsGenericParameters
            );
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
