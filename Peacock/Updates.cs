namespace Peacock;

[Mutable]
public class Updates : IUpdates
{
    public Dictionary<IControl, List<Func<IControl, IControl>>> ControlUpdates { get; } = new();
    public Dictionary<IBehavior, List<Func<IBehavior, IBehavior>>> BehaviorUpdates { get; } = new();
    public Dictionary<IModel, List<Func<IModel, IModel>>> ModelUpdates { get; } = new();
    public Dictionary<IControl, List<IBehavior>> NewBehaviors { get; } = new();

    public IUpdates StoreUpdate<T>(T key, Func<T, T> updateFunc, Dictionary<T, List<Func<T, T>>> lookup)
    {
        if (!lookup.ContainsKey(key))
            lookup.Add(key, new());
        lookup[key].Add(updateFunc);
        return this;
    }

    public IUpdates AddBehavior(IControl control, IBehavior behavior)
    {
        if (!NewBehaviors.ContainsKey(control))
            NewBehaviors.Add(control, new());
        NewBehaviors[control].Add(behavior);
        return this;
    }

    public IUpdates UpdateControl(IControl key, Func<IControl, IControl> updateFunc)
        => StoreUpdate(key, updateFunc, ControlUpdates);

    public IUpdates UpdateBehavior(IBehavior key, Func<IBehavior, IBehavior> updateFunc)
        => StoreUpdate(key, updateFunc, BehaviorUpdates);

    public IUpdates UpdateModel(IModel key, Func<IModel, IModel> updateFunc)
        => StoreUpdate(key, updateFunc, ModelUpdates);

    public IEnumerable<IControl> UpdatedControls()
        => ControlUpdates.Keys;

    public IEnumerable<IBehavior> UpdatedBehaviors()
        => BehaviorUpdates.Keys;

    public IEnumerable<IModel> UpdatedModels()
        => ModelUpdates.Keys;

    IEnumerable<IBehavior> IUpdates.NewBehaviors(IControl control)
        => NewBehaviors.ContainsKey(control) ? NewBehaviors[control] : Array.Empty<IBehavior>();

    public IControl ApplyToControl(IControl control)
        => ControlUpdates.ContainsKey(control)
            ? ControlUpdates[control].Aggregate(control, (local, func) => func(local))
            : control;

    public IBehavior ApplyToBehavior(IBehavior behavior)
        => BehaviorUpdates.ContainsKey(behavior)
            ? BehaviorUpdates[behavior].Aggregate(behavior, (local, func) => func(local))
            : behavior;

    public IModel ApplyToModel(IModel model)
        => ModelUpdates.ContainsKey(model)
            ? ModelUpdates[model].Aggregate(model, (local, func) => func(local))
            : model;
}
