using Microsoft.Extensions.Options;
using NRedisStack.RedisStackCommands;
using SaalGallery.Models.Shared;
using SaalGallery.Repository.Interfaces;
using SaalGalleryApi.Middleware;
using SaalGalleryApi.Models.Shared;
using SaalGalleryApi.Utilities.Extensions;
using StackExchange.Redis;

namespace SaalGallery.Repository;

public class GalleryRepository(IOptionsSnapshot<CustomAppSettings> customSettings,
        IDatabase Redisdb,
        ILogger<GalleryRepository> logger) : IGalleryRepository
{
    public async Task<bool> SaveRedisImage(ImageModel data, string userId)
    {
        try
        {
            var imageId = Guid.NewGuid().ToString();

            var jsonKey = customSettings.Value.RedisKeyStructure.FormatRedisKey(userId, imageId).ToLowerInvariant();
            var result = await Redisdb.JSON().SetAsync(jsonKey, "$", data);
            if (result)
                await SaveRedisImageSortedSet(imageId, userId);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, nameof(SaveRedisImage));
            return false;
        }
    }

    public async Task<bool> SaveRedisImageSortedSet(string imageId, string userId)
    {
        try
        {
            var sortedSetKey = customSettings.Value.RedisKeyStructure.FormatSortedSet(userId);
            var sortedSet = (await Redisdb.SortedSetRangeByRankWithScoresAsync(sortedSetKey)).ToList();
            if (sortedSet == null || sortedSet.Count == 0)
                sortedSet = new List<SortedSetEntry>()
                    {
                        new SortedSetEntry(new RedisValue(imageId), 0)
                    };
            else
                sortedSet.Add(new SortedSetEntry(new RedisValue(imageId), 0));

            await Redisdb.SortedSetAddAsync(sortedSetKey, sortedSet.ToArray());

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, nameof(SaveRedisImageSortedSet));
            return false;
        }
    }

    public async Task<List<ImageModel>> FetchRedisImages(string userId)
    {
        try
        {
            var sortedSetKey = customSettings.Value.RedisKeyStructure.FormatSortedSet(userId);
            var sortedSet = (await Redisdb.SortedSetRangeByRankWithScoresAsync(sortedSetKey)).ToList();

            var allImages = new List<ImageModel>();
            foreach (var item in sortedSet)
            {
                var imageId = item.Element.ToString();
                var jsonKey = customSettings.Value.RedisKeyStructure.FormatRedisKey(userId, item.Element.ToString()).ToLowerInvariant();
                var data = await Redisdb.JSON().GetAsync<ImageModel>(jsonKey);

                if (data != null)
                    allImages.Add(data);
            }

            return allImages;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, nameof(FetchRedisImages));
            return Enumerable.Empty<ImageModel>().ToList();
        }
    }
}
