namespace SaalGalleryApi.Models.Shared;

public class ImageModel
{
    public string Name { get; set; }
    public string Url { get; set; }
    public List<ImageTagModel> Tags { get; set; }
}

public class ImageTagModel
{
    public string Name { get; set; }
    public string Value { get; set; }
}
