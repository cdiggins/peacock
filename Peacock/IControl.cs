namespace Peacock;

/// <summary>
/// An immutable UI control.
/// Controls contain an immutable view object, and provide both
/// a drawing function and an input processor which returns a new
/// view object. A control manager class maintains the relationships
/// between parent-child controls and between behaviors and controls.
/// </summary>
public interface IControl : IObject
{
    IView View { get; }
    IReadOnlyList<IBehavior> Behaviors { get; }
    IReadOnlyList<IControl> Children { get; }

    ICanvas Draw(ICanvas canvas);
    IView Process(IInputEvent input);
    
    IControl With(IView view, IReadOnlyList<IBehavior> behaviors, IReadOnlyList<IControl> children);
}

public interface IUpdates
{
    IView UpdateView(IView old);
    IBehavior UpdateBehavior(IBehavior old);
    IControl UpdateControl(IControl old);
}