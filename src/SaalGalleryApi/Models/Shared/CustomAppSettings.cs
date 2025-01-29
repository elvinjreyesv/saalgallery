namespace SaalGallery.Models.Shared;

public class CustomAppSettings
{
    public string RedisKeyStructure { get; set; }
    public string RedisKeySortedSetStructure { get; set; }
    public JwtConfig JwtConfig { get; set; }
}
