namespace PipeForge.Metadata;

/// <summary>
/// A static class the contains type information for an interface.
/// This is used to avoid repetitive reflection in the code that needs these types.
/// </summary>
/// <typeparam name="TInterface"></typeparam>
internal static class InterfaceDescriptor<TInterface>
{
    internal static readonly string ArgumentExceptionMessage = "The type {0} is not an interface.";

    /// <summary>
    /// The type of the interface.
    /// </summary>
    public static readonly Type InterfaceType = typeof(TInterface);

    /// <summary>
    /// The type of a lazy instance of the interface.
    /// </summary>
    public static readonly Type LazyType = typeof(Lazy<>).MakeGenericType(InterfaceType);

    static InterfaceDescriptor()
    {
        if (!InterfaceType.IsInterface)
        {
            throw new ArgumentException(string.Format(ArgumentExceptionMessage, InterfaceType.FullName ?? InterfaceType.Name), nameof(TInterface));
        }
    }
}
