using System.Collections.Concurrent;
using PipeForge.Extensions;

namespace PipeForge.Metadata;

/// <summary>
/// Contains a concrete type and its associated interface and lazy type.
/// This is used to describe types that implement a specific interface and can be lazily instantiated.
/// The lazy type is used to defer the instantiation of the concrete type until it is actually needed.
/// </summary>
internal sealed class StepTypeDescriptor
{
    private static readonly ConcurrentDictionary<Type, Type> _lazyStepTypes = new();
    private static readonly Type _openGenericPipelineStepType = typeof(IPipelineStep<>);

    public static readonly string ArgumentExceptionMessage = "Type {0} does not implement the interface {1}.";

    /// <summary>
    /// The concrete type that implements the interface
    /// </summary>
    public Type ConcreteType { get; internal set; } = null!;

    /// <summary>
    /// The interface that the concrete type implements
    /// </summary>
    public Type InterfaceType { get; internal set; } = null!;

    /// <summary>
    /// The lazy type that can be used to register the concrete type
    /// </summary>
    public Type LazyType { get; internal set; } = null!;

    /// <summary>
    /// The name of the type, used for logging and diagnostics.
    /// </summary>
    public string TypeName { get; internal set; } = null!;

    internal StepTypeDescriptor() { }

    public static StepTypeDescriptor Create<TStepInterface>(Type type)
    {
        if (!typeof(TStepInterface).IsAssignableFrom(type))
        {
            var interfaceType = typeof(TStepInterface);
            var interfaceTypeName = interfaceType.GetTypeName();
            throw new ArgumentException(string.Format(ArgumentExceptionMessage, type.GetTypeName(), interfaceTypeName));
        }

        return new StepTypeDescriptor
        {
            ConcreteType = type,
            InterfaceType = StepInterfaceDescriptor<TStepInterface>.InterfaceType,
            LazyType = StepInterfaceDescriptor<TStepInterface>.LazyType,
            TypeName = type.GetTypeName()
        };
    }

    public static StepTypeDescriptor Create(Type type)
    {
        var interfaceType = type.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == _openGenericPipelineStepType)
            ?? throw new ArgumentException(string.Format(ArgumentExceptionMessage, type.GetTypeName(), _openGenericPipelineStepType.GetTypeName()));

        var lazyType = _lazyStepTypes.GetOrAdd(interfaceType, it => typeof(Lazy<>).MakeGenericType(it));

        return new StepTypeDescriptor
        {
            ConcreteType = type,
            InterfaceType = interfaceType,
            LazyType = lazyType,
            TypeName = type.GetTypeName()
        };
    }
}
