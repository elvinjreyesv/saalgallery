namespace SaalGallery.Mapper.Abstracts;

public interface IMapper
{
}

public interface IMapper<in TInput, out TOutput> : IMapper
{
    /// <summary>
    /// Map from an input to any object
    /// </summary>
    /// <param name="input">The input model to map</param>
    /// <param name="action">The map action</param>
    /// <returns>A model with the mapped properties</returns>
    TOutput MapFrom(TInput input, Action<IMapperContext> action);

    /// <summary>
    /// Map from an input to any object
    /// </summary>
    /// <param name="input">The input model to map</param>
    /// <returns>A model with the mapped properties</returns>
    TOutput MapFrom(TInput input);

    /// <summary>
    /// Map from a list of objects
    /// </summary>
    /// <param name="inputs">The input list of model to map</param>
    /// <param name="action">The map action</param>
    /// <returns>A model with the mapped properties</returns>
    IEnumerable<TOutput> MapFrom(IEnumerable<TInput> inputs, Action<IMapperContext> action);

    /// <summary>
    /// Map from a list of objects
    /// </summary>
    /// <param name="inputs">The input list of model to map</param>
    /// <returns>A model with the mapped properties</returns>
    IEnumerable<TOutput> MapFrom(IEnumerable<TInput> inputs);
}
