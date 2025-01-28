using SaalGalleryApi.Models.Shared;

namespace SaalGallery.Repository.Interfaces;

public interface IGalleryRepository
{
    Task<bool> SaveRedisImage(ImageModel data, string userId);
    Task<bool> SaveRedisImageSortedSet(string imageId, string userId);
    Task<List<ImageModel>> FetchRedisImages(string userId);
}
