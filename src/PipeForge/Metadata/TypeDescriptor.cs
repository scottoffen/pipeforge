using System.Collections.Concurrent;

namespace PipeForge.Metadata;

/// <summary>
/// Contains a concrete type and its associated interface and lazy type.
/// This is used to describe types that implement a specific interface and can be lazily instantiated.
/// The lazy type is used to defer the instantiation of the concrete type until it is actually needed.
/// </summary>
internal sealed class TypeDescriptor
{
    private static readonly ConcurrentDictionary<Type, Type> _lazyStepTypes = new();
    private static readonly Type _openGenericPipelineStepType = typeof(IPipelineStep<>);

    public static readonly string ArgumentExceptionMessage = "Type {0} does not implement the interface {1}.";

    /// <summary>
    /// The concrete type that implements the interface
    /// </summary>
    public Type ConcreteType { get; private set; } = null!;

    /// <summary>
    /// The interface that the concrete type implements
    /// </summary>
    public Type InterfaceType { get; private set; } = null!;

    /// <summary>
    /// The lazy type that can be used to register the concrete type
    /// </summary>
    public Type LazyType { get; private set; } = null!;

    public string TypeName { get; private set; } = null!;

    /// <summary>
    /// Creates a TypeDescriptor for a given type that implements the interface provided as a generic parameter.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if the type does not implement the specified interface.</exception>
    /// <typeparam name="TInterface"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    public static TypeDescriptor Create<TInterface>(Type type)
    {
        var interfaceType = InterfaceDescriptor<TInterface>.InterfaceType;
        if (!interfaceType.IsAssignableFrom(type))
            throw new ArgumentException(string.Format(ArgumentExceptionMessage, type.FullName ?? type.Name, interfaceType.FullName ?? interfaceType.Name));

        return new TypeDescriptor
        {
            ConcreteType = type,
            InterfaceType = InterfaceDescriptor<TInterface>.InterfaceType,
            LazyType = InterfaceDescriptor<TInterface>.LazyType,
            TypeName = type.FullName ?? type.Name ?? string.Empty
        };
    }

    public static TypeDescriptor Create(Type type)
    {
        var interfaceType = type.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == _openGenericPipelineStepType);

        if (interfaceType == null)
            throw new ArgumentException(string.Format(ArgumentExceptionMessage, type.FullName ?? type.Name, _openGenericPipelineStepType.FullName ?? _openGenericPipelineStepType.Name));

        var lazyType = _lazyStepTypes.GetOrAdd(interfaceType, it => typeof(Lazy<>).MakeGenericType(it));

        return new TypeDescriptor
        {
            ConcreteType = type,
            InterfaceType = interfaceType!,
            LazyType = lazyType,
            TypeName = type.FullName ?? type.Name ?? string.Empty
        };
    }
}
