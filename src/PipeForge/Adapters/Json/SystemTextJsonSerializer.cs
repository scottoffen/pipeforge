#if !NETSTANDARD2_0
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PipeForge.Adapters.Json;

[ExcludeFromCodeCoverage]
internal class SystemTextJsonSerializer : IJsonSerializer
{
    private static readonly SystemTextJsonSerializer _instance = new();

    private SystemTextJsonSerializer() { }

    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public string Serialize<T>(T obj) => JsonSerializer.Serialize(obj, _options);

    public T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _options)!;

    public static SystemTextJsonSerializer GetInstance() => _instance;
}
#endif
