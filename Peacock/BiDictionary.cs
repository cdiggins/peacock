namespace Peacock;

public interface IBiDictionary<T1, T2>
{
    IBiDictionary<T1, T2> Add(IEnumerable<(T1, T2)> keyValues);
    IReadOnlyList<T2> GetValues(T1 key);
    IReadOnlyList<T1> GetKeys(T2 value);
    bool HasKey(T1 key);
    bool HasValue(T2 value);
}

public class BiDictionary<T1, T2> : IBiDictionary<T1, T2>
{
    private IDictionaryOfLists<T1, T2> KeyValueLookup = new DictionaryOfLists<T1,T2>();
    private IDictionaryOfLists<T2, T1> ValueKeyLookup = new DictionaryOfLists<T2, T1>();

    public BiDictionary(IDictionaryOfLists<T1, T2> keyValueLookup, IDictionaryOfLists<T2, T1> valueKeyLookup)
        => (KeyValueLookup, ValueKeyLookup) = (keyValueLookup, valueKeyLookup);

    public IBiDictionary<T1, T2> Add(IEnumerable<(T1, T2)> keyValues)
    {
        var keyValueLookup = KeyValueLookup.Add(keyValues);
        var valueKeyLookup = ValueKeyLookup.Add(keyValues.Select(kv => (kv.Item2, kv.Item1)));
        return new BiDictionary<T1, T2>(keyValueLookup, valueKeyLookup);
    }

    public IReadOnlyList<T2> GetValues(T1 key)
        => KeyValueLookup.GetValues(key);

    public IReadOnlyList<T1> GetKeys(T2 value)
        => ValueKeyLookup.GetValues(value);

    public bool HasKey(T1 key)
        => KeyValueLookup.HasKey(key);

    public bool HasValue(T2 value)
        => ValueKeyLookup.HasKey(value);
}