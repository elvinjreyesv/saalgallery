using SaalGallery.Serializations.Abstracts;
using SaalGallery.Serializations.ContractResolvers;
using Newtonsoft.Json;

namespace SaalGallery.Serializations;

public class SerializerService : ISerializerService
{
    public T DeserializeJson<T>(string content)
    {
        return JsonConvert.DeserializeObject<T>(content);
    }

    public T DeserializeJsonWithIgnoringAttributesContractor<T>(string obj)
    {
        return JsonConvert.DeserializeObject<T>(obj, new JsonSerializerSettings()
        {
            ContractResolver = new OriginalNameContractResolver()
        });
    }

    public string SerializeAsJson<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    public string SerializeAsJsonWithIgnoringAttributesContractor<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
        {
            ContractResolver = new OriginalNameContractResolver()
        });
    }

    public T PopulateObjectWithIgnoringAttributesContractor<T>(string jsonText, T output) where T:class
    {
        JsonConvert.PopulateObject(jsonText, output, new JsonSerializerSettings() { ContractResolver = new OriginalNameContractResolver() });
        return output;
    }
}
