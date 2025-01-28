using System.Text;

namespace SaalGallery.Utilities.Formatters;

public class StringWriterUtf8 : StringWriter
{
    public override Encoding Encoding => Encoding.UTF8;
}
