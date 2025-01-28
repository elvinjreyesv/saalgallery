namespace SaalGallery.Mapper.Abstracts;

public abstract class AbstractBaseCustomMapper<TInput, TOutput> : IMapper<TInput, TOutput>
{
    protected readonly InnerMapperContext Context;

    protected AbstractBaseCustomMapper()
    {
        Context = new InnerMapperContext();
    }

    public IEnumerable<TOutput> MapFrom(IEnumerable<TInput> inputs, Action<IMapperContext> action)
    {
        Context.Items.Clear();
        action?.Invoke(Context);
        return MapFrom(inputs);
    }

    public IEnumerable<TOutput> MapFrom(IEnumerable<TInput> inputs)
    {
        return inputs?.Select(MapFrom).Where(row => row != null).ToList()
               ?? Enumerable.Empty<TOutput>();
    }

    public TOutput MapFrom(TInput input, Action<IMapperContext> action)
    {
        Context.Items.Clear();
        action?.Invoke(Context);
        return MapFrom(input);
    }

    public abstract TOutput MapFrom(TInput input);

    protected class InnerMapperContext : IMapperContext
    {
        public IDictionary<string, object> Items { get; }

        public InnerMapperContext()
        {
            Items = new Dictionary<string, object>();
        }

        public bool ContainsKey(params string[] keys)
        {
            if (keys is null || keys.Length == 0) return true;

            var result = true;
            foreach (var key in keys)
                result &= Items.ContainsKey(key);

            return result;
        }
    }
}
