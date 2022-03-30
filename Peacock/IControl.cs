namespace Peacock;

/// <summary>
/// An immutable UI control.
/// </summary>
public interface IControl
{
    IView View { get; }
    ICanvas Draw(ICanvas canvas);
    IEnumerable<IControl> GetChildren(IControlFactory factory);
    IView Process(IInputEvent input, IDispatcher dispatcher);
}

/// <summary>
/// Represents a collection of proposed changes to the store 
/// </summary>
[Mutable]
public interface IDispatcher
{
    void UpdateView(Guid id, Func<IView, IView> updateFunc);
}