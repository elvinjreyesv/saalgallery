using SaalGallery.Mapper.Abstracts;

namespace SaalGallery.Mappers.Services;

public class MappingService : IMappingService
{
    public IServiceProvider ServiceProvider { get; }

    public MappingService(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public TMapper Map<TMapper>()
    {
        var mapper = ServiceProvider.GetService<TMapper>();
        if (mapper == null)
        {
            throw new Exception($"{typeof(TMapper)}");
        }
        return mapper;
    }

    public TDestination Map<TSource, TDestination>(TSource entity)
    {
        var mapper = ServiceProvider.GetService<IMapper<TSource, TDestination>>();
        if (mapper == null)
        {
            throw new Exception($"{typeof(TSource)}, ${typeof(TDestination)}");
        }
        return mapper.MapFrom(entity);
    }

    public IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> entities)
    {
        var mapper = ServiceProvider.GetService<IMapper<TSource, TDestination>>();
        if (mapper == null)
        {
            throw new Exception($"{typeof(TSource)}, ${typeof(TDestination)}");
        }

        return entities?.Select(row => mapper.MapFrom(row)) ?? Enumerable.Empty<TDestination>();
    }
}
