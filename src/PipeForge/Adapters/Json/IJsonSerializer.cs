namespace PipeForge.Adapters;

internal interface IJsonSerializer
{
    /// <summary>
    /// Serializes an object of type T into its JSON representation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    string Serialize<T>(T obj);

    /// <summary>
    /// Deserializes a JSON string into an object of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    T Deserialize<T>(string json);
}

internal static class JsonSerializerFactory
{
    public static IJsonSerializer Create()
    {
#if NETSTANDARD2_0
        return new NewtonsoftJsonSerializer();
#else
        return new SystemTextJsonSerializer();
#endif
    }
}
