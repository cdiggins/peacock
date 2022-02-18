using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Ned;

public record Alignment(AlignmentX X, AlignmentY Y);
public record WindowProps(Rect Rect, string Title, Cursor Cursor);

public record BrushStyle(Color Color);
public record PenStyle(BrushStyle BrushStyle, double Width);
public record TextStyle(BrushStyle BrushStyle, string FontFamily, double FontSize, Alignment Alignment);
public record ShapeStyle(BrushStyle BrushStyle, PenStyle PenStyle);
public record Line(Point A, Point B);

public record Ellipse(Point Point, Radius Radius)
{
    public Ellipse(Point point, double radius)
        : this(point, new Radius(radius, radius))
    { }
}
    
public record RoundedRect(Rect Rect, Radius Radius)
{
    public RoundedRect(Rect rect)
        : this(rect, new(0,0))
    { }
}

public record StyledText(TextStyle Style, Rect Rect, string Text);
public record StyledLine(PenStyle PenStyle, Line Line);
public record StyledRect(ShapeStyle Style, RoundedRect Rect);
public record StyledEllipse(ShapeStyle Style, Ellipse Ellipse);

public record Radius(double X, double Y)
{
    public Radius(double r) : this(r, r) { }
}

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
