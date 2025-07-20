namespace PipeForge.Adapters.Json;

/// <summary>
/// Factory class to create instances of IJsonSerializer
/// </summary>
internal static class JsonSerializerFactory
{
    public static IJsonSerializer Create()
    {
#if NETSTANDARD2_0
        return NewtonsoftJsonSerializer.GetInstance();
#else
        return SystemTextJsonSerializer.GetInstance();
#endif
    }
}
