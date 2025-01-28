namespace SaalGallery.Models.Shared;

public class JwtConfig
{
    public string ValidIssuer { get; set; }
    public string ValidAudience { get; set; }
    public string SigningKey { get; set; }
}
