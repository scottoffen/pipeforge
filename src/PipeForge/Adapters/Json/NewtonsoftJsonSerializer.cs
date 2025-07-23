#if NETSTANDARD2_0
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace PipeForge.Adapters.Json;

[ExcludeFromCodeCoverage]
internal sealed class NewtonsoftJsonSerializer : IJsonSerializer
{
    private static readonly NewtonsoftJsonSerializer _instance = new();

    private NewtonsoftJsonSerializer() { }

    public string Serialize<T>(T obj) => JsonConvert.SerializeObject(obj, Formatting.Indented);

    public T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json)!;

    public static NewtonsoftJsonSerializer GetInstance() => _instance;
}
#endif
