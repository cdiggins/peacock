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
/// Represents a collection of proposed changes to the UI or application state.
/// </summary>
[Mutable]
public interface IDispatcher
{
    void AddBehavior(IView view, IBehavior behavior);
    void UpdateBehavior(IBehavior key, Func<IBehavior, IBehavior?> updateFunc);
    void UpdateView(IView key, Func<IView, IView?> updateFunc);
    void UpdateModel(IModel key, Func<IModel, IModel?> updateFunc);
}