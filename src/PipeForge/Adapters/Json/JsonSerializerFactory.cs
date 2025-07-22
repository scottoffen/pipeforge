using System.Diagnostics.CodeAnalysis;

namespace PipeForge.Adapters.Json;

[ExcludeFromCodeCoverage]
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
