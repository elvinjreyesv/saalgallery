namespace SaalGallery.Utilities.Helper;

public static class StreamHelper
{
    public static async Task<Stream> WriteStreamAsync(string body, HttpResponse response)
    {
        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream);
        await streamWriter.WriteAsync(body);
        await streamWriter.FlushAsync();
        memoryStream.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(response.Body);

        return response.Body;
    }
}