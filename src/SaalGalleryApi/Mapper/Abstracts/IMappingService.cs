namespace SaalGallery.Mapper.Abstracts;

public interface IMappingService
{
    /// <summary>
    /// Get the members that recently login
    /// </summary>
    /// <returns>A model with the members</returns>
    TMapper Map<TMapper>();

    /// <summary>
    /// Get the members that recently login
    /// </summary>
    /// <param name="bonusId">The person Id</param>
    /// <returns>A model with the members</returns>
    TDestination Map<TSource, TDestination>(TSource entity);

    /// <summary>
    /// Get the members that recently login
    /// </summary>
    /// <param name="bonusId">The person Id</param>
    /// <returns>A model with the members</returns>
    IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> entities);
}
