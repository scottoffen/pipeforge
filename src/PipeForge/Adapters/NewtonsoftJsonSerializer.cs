#if NETSTANDARD2_0
using Newtonsoft.Json;

namespace PipeForge.Adapters;

internal class NewtonsoftJsonSerializer : IJsonSerializer
{
    public string Serialize<T>(T obj) => JsonConvert.SerializeObject(obj, Formatting.Indented);

    public T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json)!;
}
#endif
