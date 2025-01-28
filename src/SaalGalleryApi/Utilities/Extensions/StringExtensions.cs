using SaalGalleryApi.Utilities.Constants;

namespace SaalGalleryApi.Utilities.Extensions;

public static class StringExtensions
{
    public static string FormatRedisKey(this string redisKeyStructure, string userId, string key)
        => redisKeyStructure.Replace(GenericConstants.UserId, userId).Replace(GenericConstants.Key, key);

    public static string FormatSortedSet(this string redisKeyStructure, string userId)
       => redisKeyStructure.Replace(GenericConstants.UserId, userId);
}
