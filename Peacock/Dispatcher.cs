namespace Peacock;

[Mutable]
public class Dispatcher : IDispatcher
{
    public Dictionary<Guid, List<Func<IView, IView>>> _lookup { get; } = new();

    public void UpdateView(Guid id, Func<IView, IView> updateFunc)
    {
        if (!_lookup.ContainsKey(id))
        {
            _lookup.Add(id, new());
        }   
        _lookup[id].Add(updateFunc);
    }
}