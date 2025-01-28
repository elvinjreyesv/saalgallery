using SaalGalleryApi.Models.Shared;

namespace SaalGallery.Services.Interfaces;

public interface IGalleryService
{
    Task<bool> UploadImage(byte[] imageContent);
    Task<bool> DeleteAllImages();
    Task<bool> DeleteImage(string image);
    Task<List<ImageModel>> ListAllUserImages();
}
