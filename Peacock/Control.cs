using System.Windows;

namespace Peacock;

/// <summary>
/// This is a default implementation of IControl that can be used as-is, or also serve as a base class for other controls.
/// </summary>
public record Control<TView>(Rect Dimensions, TView View, IReadOnlyList<IControl> Children, Func<IUpdates, IControl, IControl, IUpdates> Callback) : IControl 
    where TView : IView
{
    IView IControl.View => View;
    public virtual ICanvas Draw(ICanvas canvas) => canvas;
    public virtual IUpdates Process(IInputEvent input, IUpdates updates) => updates;
    public virtual IEnumerable<IBehavior> GetDefaultBehaviors() => Enumerable.Empty<IBehavior>();
    public static IUpdates DefaultCallback(IUpdates updates, IControl oldControl, IControl newControl) => updates;

    public static IReadOnlyList<IControl> ToChildren(params IControl?[] controls)
        => controls.Where(c => c != null).OfType<IControl>().ToList();

    public static IReadOnlyList<IControl> ToChildren(params IEnumerable<IControl>[] controlLists)
        => controlLists.SelectMany(c => c).ToList();

    public static IControl Default = new EmptyControl();
}

public class EmptyView : IView
{
    public object Id => Guid.NewGuid();

    public static EmptyView Default = new();
}

public record EmptyControl() : Control<EmptyView>(
    Rect.Empty, 
    EmptyView.Default, 
    Array.Empty<IControl>(), 
    DefaultCallback)
{
}
