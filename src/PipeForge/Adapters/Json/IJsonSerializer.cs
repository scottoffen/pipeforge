namespace PipeForge.Adapters.Json;

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
