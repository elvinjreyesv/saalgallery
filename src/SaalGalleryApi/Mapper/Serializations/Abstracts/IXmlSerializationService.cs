namespace SaalGallery.Mapper.Serializations.Abstracts;

public interface IXmlSerializationService
{
    T Deserialize<T>(string input) where T : class;
    T DeserializeWithEmptySchema<T>(string input) where T : class;

    string Serialize<T>(T input);
}
