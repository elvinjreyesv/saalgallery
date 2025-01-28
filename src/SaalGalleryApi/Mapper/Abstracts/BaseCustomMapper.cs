using SaalGallery.Serializations.Abstracts;

namespace SaalGallery.Mapper.Abstracts;

public abstract class BaseCustomMapper<TInput, TOutput> : AbstractBaseCustomMapper<TInput, TOutput>
{
    protected IMappingService MappingService { get; }
    protected ISerializerService SerializerService { get; }

    protected BaseCustomMapper(IMappingService mappingService, ISerializerService serializerService)
    {
        MappingService = mappingService;
        SerializerService = serializerService;
    }

    protected TParamOutput InitialTransformation<TParamInput, TParamOutput>(TParamInput input) where TParamOutput:class, new()
    {
        var asJson = SerializerService.SerializeAsJsonWithIgnoringAttributesContractor(input);
        var output = SerializerService.PopulateObjectWithIgnoringAttributesContractor(asJson, new TParamOutput());
        return output;
    }
}
