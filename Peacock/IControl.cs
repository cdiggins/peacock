namespace Peacock;

/// <summary>
/// An immutable UI control 
/// </summary>
public interface IControl
{
    public IView View { get; }
    public double ZOrder { get; }
    public IReadOnlyList<IControl> Children { get; }
    public IReadOnlyList<IBehavior> Behaviors { get; }
    public ICanvas Draw(ICanvas canvas);
    public (IControl, IUpdates) ProcessInput(IUpdates updates, InputEvent input);
    public IControl AddBehavior(IBehavior behavior);
    public IControl RemoveBehavior(IBehavior behavior);
    public IControl UpdateView(IView view);
}