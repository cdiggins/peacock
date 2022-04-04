namespace Peacock;

/// <summary>
/// This is a default implementation of IControl that can be used as-is, or also serve as a base class for other controls.
/// </summary>
public record Control<TView>(TView View, Func<IUpdates, IControl, IUpdates> Callback, List<IBehavior>? Behaviors = null) : IControl 
    where TView : IView
{
    IView IControl.View => View;
    public virtual ICanvas Draw(ICanvas canvas) => canvas;
    public virtual IEnumerable<IControl> GetChildren(IControlFactory factory) => Enumerable.Empty<IControl>();
    public virtual IUpdates Process(IInputEvent input, IUpdates updates) => updates;
    public virtual IEnumerable<IBehavior> GetDefaultBehaviors() => Behaviors ?? Enumerable.Empty<IBehavior>();
}