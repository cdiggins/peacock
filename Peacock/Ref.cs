namespace Peacock;

public record RefList<T>(IReadOnlyList<Ref<T>> Refs) where T : IObject
{
    public int Count => Refs.Count;
    public Ref<T> this[int index] => Refs[index];
    public IEnumerable<T> Get(IObjectStore store) => Refs.Select(r => r.Get(store));
    public static readonly RefList<T> Empty = new(Array.Empty<Ref<T>>());
}

public record Ref<T>(Guid Id) where T: IObject
{
    public T Get(IObjectStore store) => (T)store.Get(Id);
    public static implicit operator Ref<T>?(T? obj) => obj == null ? null : new(obj.Id);
    public static readonly Ref<T> Empty = new(Guid.Empty);
}

public static class RefExtensions
{
    public static Ref<T> ToRef<T>(this T self) where T : IObject => new(self.Id);
    public static RefList<T> ToRefList<T>(this IEnumerable<T> self) where T : IObject => new(self.Select(ToRef).ToList());
}
