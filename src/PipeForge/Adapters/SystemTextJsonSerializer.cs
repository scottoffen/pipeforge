#if !NETSTANDARD2_0
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PipeForge.Adapters;

internal class SystemTextJsonSerializer : IJsonSerializer
{
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public string Serialize<T>(T obj) => JsonSerializer.Serialize(obj, _options);

    public T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _options)!;
}
#endif
