namespace Peacock;

public interface IDictionaryOfLists<T1, T2>
{
    IDictionaryOfLists<T1, T2> Add(IEnumerable<(T1, T2)> keyValues);
    IDictionaryOfLists<T1, T2> Remove(IEnumerable<T1> keys);
    IReadOnlyList<T2> GetValues(T1 key);
    IEnumerable<T1> GetKeys();
    bool HasKey(T1 key);
}

public class DictionaryOfLists<T1, T2> : IDictionaryOfLists<T1, T2>
    where T1 : notnull
{
    private readonly Dictionary<T1, List<T2>> _lookup = new Dictionary<T1, List<T2>>();

    public DictionaryOfLists()
    {}

    public DictionaryOfLists(Dictionary<T1, List<T2>> values)
        => _lookup = values;

    public DictionaryOfLists(IEnumerable<(T1, T2)> keyValues)
        => AddValues(_lookup, keyValues);

    public static void AddValues(Dictionary<T1, List<T2>> lookup, IEnumerable<(T1, T2)> keyValues)
    {
        foreach (var (key, value) in keyValues)
        {
            if (!lookup.ContainsKey(key))
                lookup.Add(key, new List<T2> { value });
            else
                lookup[key].Add(value);
        }
    }

    public IDictionaryOfLists<T1, T2> Add(IEnumerable<(T1, T2)> keyValues)
    {
        var d = new Dictionary<T1, List<T2>>(_lookup);
        AddValues(_lookup, keyValues);
        return new DictionaryOfLists<T1, T2>(d);
    }

    public IDictionaryOfLists<T1, T2> Remove(IEnumerable<T1> keys)
    {
        var d = new Dictionary<T1, List<T2>>(_lookup);
        foreach (var k in keys)
            d.Remove(k);
        return new DictionaryOfLists<T1, T2>(d);
    }

    public IEnumerable<T1> GetKeys()
        => _lookup.Keys;

    public IReadOnlyList<T2> GetValues(T1 key)
        => _lookup[key];

    public bool HasKey(T1 key)
        => _lookup.ContainsKey(key);
}

