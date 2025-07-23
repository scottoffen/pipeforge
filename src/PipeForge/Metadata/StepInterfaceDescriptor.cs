using PipeForge.Extensions;

namespace PipeForge.Metadata;

/// <summary>
/// A static class the contains type information for an interface.
/// This is used to avoid repetitive reflection in the code that needs these types.
/// </summary>
/// <typeparam name="TStepInterface"></typeparam>
internal static class StepInterfaceDescriptor<TStepInterface>
{
    internal static readonly string MessageArgumentException = "The type {0} is not an interface.";

    internal static readonly string MessageNotPipelineStep = "The type {0} does not implement the IPipelineStep interface.";

    /// <summary>
    /// The type of the interface.
    /// </summary>
    public static readonly Type InterfaceType = typeof(TStepInterface);

    /// <summary>
    /// The type of a lazy instance of the interface.
    /// </summary>
    public static readonly Type LazyType = typeof(Lazy<>).MakeGenericType(InterfaceType);

    static StepInterfaceDescriptor()
    {
        var name = InterfaceType.GetTypeName();

        if (!InterfaceType.IsInterface)
        {
            throw new ArgumentException(string.Format(MessageArgumentException, name), nameof(TStepInterface));
        }

        if (!InterfaceType.ImplementsPipelineStep())
        {
            throw new ArgumentException(string.Format(MessageNotPipelineStep, name), nameof(TStepInterface));
        }
    }
}
