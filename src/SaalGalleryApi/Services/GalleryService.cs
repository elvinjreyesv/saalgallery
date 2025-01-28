using MetadataExtractor.Formats.Exif;
using MetadataExtractor;
using SaalGallery.Repository.Interfaces;
using SaalGallery.Services.Interfaces;
using SaalGalleryApi.Models.Shared;
using Supabase.Storage;

namespace SaalGallery.Services;

public class GalleryService(Supabase.Client supabase,
        IHttpContextAccessor httpContextAccessor,
        IGalleryRepository galleryRepository,
        ILogger<GalleryService> logger) : IGalleryService
{
    private readonly string _userId = httpContextAccessor.HttpContext.Items["UserId"] as string;

    public async Task<bool> UploadImage(byte[] imageContent, string imagePath, string imageName)
    {
        try
        {
            var bucket = await supabase.Storage.GetBucket(_userId);
            if (bucket == null)
            {
                await supabase.Storage.CreateBucket(_userId);
                await supabase.Storage.UpdateBucket(_userId, new BucketUpsertOptions { Public = true });
            }

            var image = await supabase.Storage
              .From(_userId)
              .Upload(imagePath, imageName, new Supabase.Storage.FileOptions 
              { 
                  CacheControl = "3600",
                  Upsert = false 
              });

            var imageDetails = new ImageModel()
            {
                Url = image,
                Name = imageName,
                Tags = ImageTags(imageContent)
            };

            return await galleryRepository.SaveRedisImage(imageDetails, _userId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, nameof(UploadImage));
            return false;
        }
    }

    public async Task<bool> DeleteAllImages()
    {
        try
        {
            var objects = await supabase.Storage.From(_userId).List();

            if(objects == null || objects.Count == 0)
                return true;

            var allImages = objects.Select(x => x.Name).ToList();
            await supabase.Storage.From(_userId).Remove(allImages!);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, nameof(DeleteAllImages));
            return false;
        }
    }

    public async Task<bool> DeleteImage(string image)
    {
        try
        {
            await supabase.Storage.From(_userId).Remove(new List<string>() { image });

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, nameof(DeleteImage));
            return false;
        }
    }

    public async Task<List<ImageModel>> ListAllUserImages()
        => await galleryRepository.FetchRedisImages(_userId);

    private static List<ImageTagModel> ImageTags(byte[] imageContent)
    {
        var tags = new List<ImageTagModel>();
        var directories = ImageMetadataReader.ReadMetadata(new MemoryStream(imageContent));

        var exifDirectory = directories.OfType<ExifDirectoryBase>().FirstOrDefault();
        if (exifDirectory != null)
        {
            var date = exifDirectory.GetDescription(ExifDirectoryBase.TagDateTime);
            if (!string.IsNullOrEmpty(date))
                tags.Add(new ImageTagModel
                {
                    Name = "Date",
                    Value = date
                });

            // Extract other possible characteristics from EXIF
            var cameraModel = exifDirectory.GetDescription(ExifDirectoryBase.TagModel);
            if (!string.IsNullOrEmpty(cameraModel))
                tags.Add(new ImageTagModel
                {
                    Name = "Camera Model",
                    Value = cameraModel
                });

            var cameraMake = exifDirectory.GetDescription(ExifDirectoryBase.TagMake);
            if (!string.IsNullOrEmpty(cameraMake))
                tags.Add(new ImageTagModel
                {
                    Name = "Camera Make",
                    Value = cameraMake
                });
        }

        return tags;
    }
}
