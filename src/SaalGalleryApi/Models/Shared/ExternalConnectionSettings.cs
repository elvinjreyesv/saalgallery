namespace SaalGalleryApi.Models.Shared;

public class ExternalConnectionSettings
{
    public SupabaseSettings SupabaseConnection { get; set; }
    public RedisSettings RedisSaalDB { get; set; }
}
