using System.Windows;

namespace Peacock;

/// <summary>
/// An immutable UI control. A control generates child controls on demand, but does not maintain a list. 
/// </summary>
public interface IControl
{
    IView View { get; }
    Rect Dimensions { get; }
    Func<IUpdates, IControl, IControl, IUpdates> Callback { get; }
    ICanvas Draw(ICanvas canvas);
    IEnumerable<IControl> GetChildren(IControlFactory factory);
    IEnumerable<IBehavior> GetDefaultBehaviors();
    IUpdates Process(IInputEvent input, IUpdates updates);
}