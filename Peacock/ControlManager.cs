namespace Peacock;

/// <summary>
/// This is the work-horse of the UI system. It manages the state of the UI.
/// It holds the list of current controls, their parent-child relationships,
/// and the behaviors.
/// </summary>
public record ControlManager(
    IControl Root,
    IControlFactory Factory,
    IObjectStore Controls,
    IObjectStore Behaviors, 
    IRelationStore ControlRelations,
    IRelationStore BehaviorRelations)
{
    public IEnumerable<IControl> GetControls(IEnumerable<Guid> ids)
        => ids.Select(id => Controls.Get<IControl>(id));

    public IEnumerable<IBehavior> GetBehaviors(IEnumerable<Guid> ids)
        => ids.Select(id => Behaviors.Get<IBehavior>(id));

    public IEnumerable<Guid> GetChildrenIds(Guid id)
        => ControlRelations.GetRelationsFrom(id).Select(r => r.B);

    public IEnumerable<IControl> GetChildren(Guid id)
        => GetControls(GetChildrenIds(id));

    public IEnumerable<IControl> GetChildren(IControl control)
        => GetChildren(control.Id);

    public IEnumerable<IBehavior> GetBehaviors(Guid id)
        => BehaviorRelations.GetRelationsFrom(id).Select(r => Behaviors.Get<IBehavior>(r.A));

    public IEnumerable<IBehavior> GetBehaviors(IControl control)
        => GetBehaviors(control.Id);

    public ControlManager AddBehaviors(IControl control, IEnumerable<IBehavior> behaviors)
        => this with
        {
            Behaviors = Behaviors.Add(behaviors),
            BehaviorRelations = BehaviorRelations.Add(behaviors.Select(b => new Relation(control.Id, b.Id)))
        };

    public ControlManager RemoveBehavior(IBehavior behavior)
        => this with
        {
            Behaviors = Behaviors.Remove(behavior.Id),
            BehaviorRelations = BehaviorRelations.Remove(behavior.Id),
        };

    public ControlManager RemoveBehavior(Guid id)
        => this with
        {
            Behaviors = Behaviors.Remove(id),
            BehaviorRelations = BehaviorRelations.Remove(id),
        };

    public ControlManager RemoveBehaviors(IEnumerable<Guid> ids)
        => this with
        {
            Behaviors = Behaviors.Remove(ids),
            BehaviorRelations = BehaviorRelations.Remove(ids),
        };

    public ControlManager RemoveBehaviors(IControl control)
        => RemoveBehaviors(GetBehaviors(control).Select(b => b.Id));

    public ControlManager RemoveParentOrChildRelations(IEnumerable<Guid> ids)
        => this with { ControlRelations = ControlRelations.Remove(ids) };

    public ControlManager RemoveChildRelations(Guid parentId)
        => this with { ControlRelations = ControlRelations.Where(r => r.A == parentId) };

    public ControlManager RemoveChildren(Guid parentId)
        => RemoveControls(GetChildren(parentId).Select(c => c.Id)).RemoveChildRelations(parentId);

    public ControlManager AddChildRelations(Guid parentId, IEnumerable<Guid> childIds)
        => this with { ControlRelations = ControlRelations.Add(childIds.Select(c => new Relation(parentId, c))) };

    public ControlManager AddControls(IEnumerable<IControl> controls)
        => this with { Controls = Controls.Add(controls) };

    public ControlManager AddChild(Guid parentId, IControl child)
        => AddChildren(parentId, new[] { child });

    public ControlManager AddChildren(Guid parentId, IEnumerable<IControl> children)
        => AddControls(children).AddChildRelations(parentId, children.Select(c => c.Id));

    public ControlManager RemoveControls(IEnumerable<Guid> ids)
        => (this with { Controls = Controls.Remove(ids) }).RemoveParentOrChildRelations(ids);

    public HashSet<Guid> GetSubGraphIds(IEnumerable<Guid> ids, HashSet<Guid>? result = null)
    {
        result ??= new HashSet<Guid>();
        foreach (var id in ids)
        {
            if (result.Contains(id))
                continue;
            result.Add(id);
            result = GetSubGraphIds(GetChildrenIds(id), result);
        }

        return result;
    }
}

public static class ControlManagerExtensions
{
    public static ICanvas Draw(this ICanvas canvas, ControlManager manager)
        => canvas.Draw(manager, manager.Root);

    public static ICanvas Draw(this ICanvas canvas, ControlManager manager, IControl control)
        => manager.GetChildren(control).Aggregate(control.Draw(canvas),
            (localCanvas, localControl) => localCanvas.Draw(manager, localControl));
}