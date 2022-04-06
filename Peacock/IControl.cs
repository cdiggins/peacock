using System.Diagnostics.Metrics;
using System.Windows;

namespace Peacock;

/// <summary>
/// The absolute position,
/// </summary>
public record Measurements(Point Position, Vector Offset, Size Size)
{
    public Measurements(Measurements parent, Vector offset, Size size)
        : this(parent.Position + offset, offset, size)
    { }

    public Measurements(Measurements parent, Vector offset)
        : this(parent, offset, parent.Size) { }

    public Measurements()
        : this(new Point(), new Vector(), Size.Empty)
    { }

    public Rect AbsoluteRect
        => new(Position, Size);

    public Rect RelativeRect
        => new(new Point(Offset.X, Offset.Y), Size);

    public Measurements Relative(Vector offset)
        => new(Position, offset, Size);

    public Measurements Relative()
        => Relative(new());

    public static implicit operator Measurements(Rect rect)
        => new(rect.TopLeft, new(), rect.Size);

    public static Measurements Default = new();
}

/// <summary>
/// An immutable UI control. A control generates child controls on demand, but does not maintain a list. 
/// </summary>
public interface IControl
{
    IView View { get; }
    Measurements Measurements { get; }
    Func<IUpdates, IControl, IControl, IUpdates> Callback { get; }
    ICanvas Draw(ICanvas canvas);
    IReadOnlyList<IControl> Children { get; }
    IEnumerable<IBehavior> GetDefaultBehaviors();
    IUpdates Process(IInputEvent input, IUpdates updates);
}

public static class ControlExtensions
{
    public static IEnumerable<IControl> Descendants(this IControl control)
    {
        foreach (var child in control.Children)
        {
            foreach (var d in child.Descendants())
                yield return d;
        }  
        yield return control;
    }
}