namespace Peacock;

public interface IObjectStore
{
    IObject Get(Guid id);
    bool Contains(Guid id);
    IObjectStore Add(IEnumerable<IObject> objects);
    IObjectStore Remove(IEnumerable<Guid> id);
    IEnumerable<IObject> GetObjects();
}

public record Relation(Guid A, Guid B);

public interface IRelationStore
{
    IRelationStore Add(IEnumerable<Relation> relations);
    IRelationStore Where(Func<Relation, bool> filter);
    IEnumerable<Relation> GetRelations();
}

public record ObjectStore
    : IObjectStore
{
    public IReadOnlyDictionary<Guid, IObject>? Objects { get; init; }

    public ObjectStore(IReadOnlyDictionary<Guid, IObject>? objects = null)
        => Objects = objects ?? new Dictionary<Guid, IObject>();

    public IObjectStore Add(IEnumerable<IObject> objects)
        => this with { Objects = Objects.Add(objects.Select(obj => (obj.Id, obj))) };

    public IObjectStore Remove(IEnumerable<Guid> ids)
        => this with { Objects = Objects.Remove(ids) };

    public bool Contains(Guid id)
        => Objects.ContainsKey(id);

    public IObject Get(Guid id)
        => Objects[id];

    public IEnumerable<IObject> GetObjects()
        => Objects.Values;
}

public record RelationStore(IReadOnlyList<Relation> Relations)
    : IRelationStore
{
    public IRelationStore Add(IEnumerable<Relation> relations)
        => this with { Relations = Relations.Concat(relations).Distinct().ToList() };

    public IRelationStore Where(Func<Relation, bool> predicate)
        => this with { Relations = Relations.Where(predicate).ToList() };

    public IEnumerable<Relation> GetRelations()
        => Relations;
}

public static class ObjectStoreExtensions
{
    public static T Get<T>(this IObjectStore self, Guid id) where T : IObject
        => (T)self.Get(id);

    public static IObjectStore Add(this IObjectStore store, IObject obj)
        => store.Add(new[] { obj });

    public static IObjectStore Remove(this IObjectStore store, Guid id)
        => store.Remove(new[] { id });

    public static IEnumerable<(T1, T2)> GetKeyValues<T1, T2>(this IDictionaryOfLists<T1, T2> self)
        => self.GetKeys().SelectMany(k => self.GetValues(k).Select(v => (k, v)));

    public static IDictionaryOfLists<T1, T2> Add<T1, T2>(this IDictionaryOfLists<T1, T2> self,
        IEnumerable<(T1, T2)> values)
        => self.Add(self.GetKeyValues().Concat(values));

    public static IReadOnlyDictionary<TKey, TValue> Add<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> self,
        IEnumerable<(TKey, TValue)> values)
    {
        var d = new Dictionary<TKey, TValue>();
        foreach (var kv in self)
            d[kv.Key] = kv.Value;
        foreach (var kv in values)
            d[kv.Item1] = kv.Item2;
        return d;
    }

    public static IReadOnlyDictionary<TKey, TValue> Remove<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> self,
        IEnumerable<TKey> keys)
    {
        var d = new Dictionary<TKey, TValue>();
        foreach (var kv in self)
            d.Add(kv.Key, kv.Value);
        foreach (var k in keys)
            d.Remove(k);
        return d;
    }

    public static IRelationStore Add(this IRelationStore store, Relation relation)
        => store.Add(new[] { relation });

    public static IRelationStore Add(this IRelationStore store, Guid a, Guid b)
        => store.Add(new Relation(a, b));

    public static IRelationStore Remove(this IRelationStore store, Relation r)
        => store.Where(x => !x.Equals(r));

    public static IRelationStore Remove(this IRelationStore store, IEnumerable<Guid> ids)
        => store.Remove(ids.ToHashSet());

    public static IRelationStore Remove(this IRelationStore store, HashSet<Guid> ids)
        => store.Where(r => !ids.Contains(r.A) && !ids.Contains(r.B));

    public static IRelationStore Remove(this IRelationStore store, Guid id)
        => store.Remove(new[] { id });

    public static IEnumerable<Relation> GetRelationsFrom(this IRelationStore store, Guid id)
        => store.GetRelations().Where(r => r.A == id);

    public static IEnumerable<Relation> GetRelationsTo(this IRelationStore store, Guid id)
        => store.GetRelations().Where(r => r.B == id);

    public static IEnumerable<Relation> GetRelationsWith(this IRelationStore store, Guid id)
        => store.GetRelations().Where(r => r.A == id || r.B == id);
}

