#if NETSTANDARD2_0
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace PipeForge.Adapters.Json;

[ExcludeFromCodeCoverage]
internal sealed class NewtonsoftJsonSerializer : IJsonSerializer
{
    private static NewtonsoftJsonSerializer instance = new();

    private NewtonsoftJsonSerializer() { }

    public string Serialize<T>(T obj) => JsonConvert.SerializeObject(obj, Formatting.Indented);

    public T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json)!;

    public static NewtonsoftJsonSerializer GetInstance() => instance;
}
#endif
