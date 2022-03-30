using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Peacock;

public record Alignment(AlignmentX X, AlignmentY Y)
{
    public static Alignment LeftCenter => new(AlignmentX.Left, AlignmentY.Center);
    public static Alignment CenterCenter => new(AlignmentX.Center, AlignmentY.Center);
    public static Alignment RightCenter => new(AlignmentX.Right, AlignmentY.Center);
    public static Alignment LeftTop => new(AlignmentX.Left, AlignmentY.Top);
    public static Alignment CenterTop => new(AlignmentX.Center, AlignmentY.Top);
    public static Alignment RightTop => new(AlignmentX.Right, AlignmentY.Top);
    public static Alignment LeftBottom => new(AlignmentX.Left, AlignmentY.Bottom);
    public static Alignment CenterBottom => new(AlignmentX.Center, AlignmentY.Bottom);
    public static Alignment RightBottom => new(AlignmentX.Right, AlignmentY.Bottom);
}

public record WindowProps(Rect Rect, string Title, Cursor Cursor);

public record BrushStyle(Color Color)
{
    public static implicit operator BrushStyle(Color color) => new(color);
    public static BrushStyle Empty = Colors.Transparent;
}

public record PenStyle(BrushStyle BrushStyle, double Width)
{
    public static implicit operator PenStyle(Color color) => new(color, 1);
    public static implicit operator PenStyle(BrushStyle brush) => new(brush, 1);
    public static PenStyle Empty = Colors.Transparent;
}

public record TextStyle(BrushStyle BrushStyle, string FontFamily, double FontSize, Alignment Alignment);
public record ShapeStyle(BrushStyle BrushStyle, PenStyle PenStyle);
public record Line(Point A, Point B);
public record Ellipse(Point Point, Radius Radius);

public record RoundedRect(Rect Rect, Radius Radius)
{
    public RoundedRect(Rect rect) 
        : this(rect, new(0,0)) { }
}

public record StyledText(TextStyle Style, Rect Rect, string Text);
public record StyledLine(PenStyle PenStyle, Line Line);
public record StyledRect(ShapeStyle Style, RoundedRect Rect);
public record StyledEllipse(ShapeStyle Style, Ellipse Ellipse);

public record Radius(double X, double Y)
{
    public Radius(double r) : this(r, r) { }
}

/// <summary>
/// A drawing abstraction.
/// This can be used to support different drawing platforms. 
/// </summary>
public interface ICanvas
{
    ICanvas Draw(StyledText text);
    ICanvas Draw(StyledLine line);
    ICanvas Draw(StyledEllipse ellipse);
    ICanvas Draw(StyledRect rect);
    ICanvas Draw(BrushStyle brushStyle, PenStyle penStyle, Geometry geometry);
    Size MeasureText(StyledText text);
    ICanvas SetRect(Rect rect);
    ICanvas PopRect();
}

