using SaalGallery.Mapper.Serializations.Abstracts;
using System.Xml.Serialization;
using System.Xml;
using SaalGallery.Utilities.Formatters;

namespace SaalGallery.Mapper.Serializations;

public class XmlSerializationService : IXmlSerializationService
{
    public T Deserialize<T>(string input) where T : class
    {
        XmlSerializer ser = new XmlSerializer(typeof(T));

        using (StringReader sr = new StringReader(input))
        {
            return (T)ser.Deserialize(sr);
        }
    }

    public T DeserializeWithEmptySchema<T>(string input) where T : class
    {
        XmlSerializer ser = new XmlSerializer(typeof(T));

        using (StringReader sr = new StringReader(input))
        {
            return (T)ser.Deserialize(new IgnoreNamespaceXmlTextReader(sr));
        }
    }

    public string Serialize<T>(T input)
    {
        var xns = new XmlSerializerNamespaces();
        xns.Add(string.Empty, string.Empty);

        XmlSerializer xmlSerializer = new XmlSerializer(input.GetType());

        using (StringWriter textWriter = new StringWriterUtf8())
        {
            xmlSerializer.Serialize(textWriter, input, xns);
            return textWriter.ToString();
        }
    }

    public class IgnoreNamespaceXmlTextReader : XmlTextReader
    {
        public IgnoreNamespaceXmlTextReader(TextReader reader) : base(reader)
        {
        }

        public override string NamespaceURI => "";
    }
}
