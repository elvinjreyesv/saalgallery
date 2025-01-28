namespace SaalGallery.Mapper.Abstracts;

public interface IMapperContext
{
    /// <summary>
    /// Return the mapped item
    /// </summary>
    /// <returns>A dictionary of the mapped model items</returns>
    IDictionary<string, object> Items { get; }
}
