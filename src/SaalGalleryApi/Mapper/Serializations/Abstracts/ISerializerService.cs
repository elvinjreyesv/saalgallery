namespace SaalGallery.Serializations.Abstracts;

public interface ISerializerService
{
    /// <summary>
    /// Serialize a generic object to json
    /// </summary>
    /// <param name="obj">The input object to serialize</param>
    /// <returns>A string with the serialized object</returns>
    string SerializeAsJson<T>(T obj);

    /// <summary>
    /// Serialize a generic object to json and ignore attributes
    /// </summary>
    /// <param name="obj">The input object to serialize</param>
    /// <returns>A string with the serialized object</returns>
    string SerializeAsJsonWithIgnoringAttributesContractor<T>(T obj);

    /// <summary>
    /// Deserialize a string object to json
    /// </summary>
    /// <param name="content">The input object to deserialize</param>
    /// <returns>The deserialized generic object</returns>
    T DeserializeJson<T>(string content);

    /// <summary>
    /// Deserialize a string object to json and ignore attributes
    /// </summary>
    /// <param name="obj">The input object to deserialize</param>
    /// <returns>The deserialized generic object</returns>
    T DeserializeJsonWithIgnoringAttributesContractor<T>(string obj);

    /// <summary>
    /// Deserialize a string object to json and ignore attributes
    /// </summary>
    /// <param name="jsonText">The input object to deserialize</param>
    /// <param name="output">The output object type</param>
    /// <returns>The deserialized generic object</returns>
    T PopulateObjectWithIgnoringAttributesContractor<T>(string jsonText, T output) where T : class;
}
