namespace Peacock;

[Mutable]
public class Dispatcher : IDispatcher
{
    public Dictionary<IView, List<Func<IView, IView?>>> ViewUpdates { get; } = new();
    public Dictionary<IBehavior, List<Func<IBehavior, IBehavior?>>> BehaviorUpdates { get; } = new();
    public Dictionary<IModel, List<Func<IModel, IModel?>>> ModelUpdates { get; } = new();
    public Dictionary<IView, List<IBehavior>> NewBehaviors { get; } = new();

    public static void StoreUpdate<T>(T key, Func<T, T> updateFunc, Dictionary<T, List<Func<T, T?>>> lookup)
    {
        if (!lookup.ContainsKey(key))
            lookup.Add(key, new());
        lookup[key].Add(updateFunc);
    }

    public void AddBehavior(IView view, IBehavior behavior)
    {
        if (!NewBehaviors.ContainsKey(view))
            NewBehaviors.Add(view, new());
        NewBehaviors[view].Add(behavior);
    }

    public void UpdateView(IView key, Func<IView, IView?> updateFunc)
        => StoreUpdate(key, updateFunc, ViewUpdates);

    public void UpdateBehavior(IBehavior key, Func<IBehavior, IBehavior?> updateFunc)
        => StoreUpdate(key, updateFunc, BehaviorUpdates);

    public void UpdateModel(IModel key, Func<IModel, IModel?> updateFunc)
        => StoreUpdate(key, updateFunc, ModelUpdates);
}