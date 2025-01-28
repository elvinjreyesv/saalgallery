namespace OpenTelemetryTool.Serialization.Interface;

public interface ISerializer
{
    Task<string> SerializeAsync<T>(T x);

    Task<T> DeserializeAsync<T>(string x);

    string Serialize<T>(T x);

    T Deserialize<T>(string x);
}
