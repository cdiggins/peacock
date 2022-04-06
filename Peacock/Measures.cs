using System.Windows;

namespace Peacock;

/// <summary>
/// The absolute position,
/// </summary>
public record Measures(Point Position, Vector Offset, Size Size)
{
    public Measures(Point Parent, Rect OffsetRect)
        : this(Parent, new Vector(OffsetRect.Left, OffsetRect.Top), OffsetRect.Size) 
    { }

    public Measures(Measures parent, Vector offset, Size size)
        : this(parent.Position + offset, offset, size)
    { }

    public Measures(Measures parent, Vector offset)
        : this(parent, offset, parent.Size) { }

    public Measures()
        : this(new Point(), new Vector(), Size.Empty)
    { }

    public Rect AbsoluteRect
        => new(Position, Size);

    public Rect RelativeRect
        => new(new Point(Offset.X, Offset.Y), Size);

    public Measures Relative(Size size)
        => new(Position, new(), size);

    public Measures Relative(Vector offset, Size size)
        => new(Position, offset, size);

    public Measures Relative(Rect rect)
        => Relative(new Vector(rect.Left, rect.Right), rect.Size);

    public Measures Relative(Vector offset)
        => new(Position, offset, Size);

    public Measures Relative()
        => Relative(new Vector());

    public static Measures Default = new();
}