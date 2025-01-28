using Newtonsoft.Json;
using OpenTelemetryTool.Serialization.Interface;

namespace OpenTelemetryTool.Serialization;

public class NewtonsoftJsonSerializer : ISerializer
{
    private JsonSerializerSettings Settings { get; } = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore };
    public T Deserialize<T>(string x)
        => JsonConvert.DeserializeObject<T>(x)!;

    public Task<T> DeserializeAsync<T>(string x)
        => Task.FromResult(Deserialize<T>(x)!);

    public string Serialize<T>(T x)
        => JsonConvert.SerializeObject(x, Settings);

    public Task<string> SerializeAsync<T>(T x)
        => Task.FromResult(Serialize(x));
}
