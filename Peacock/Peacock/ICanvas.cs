using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Peacock
{
    public record Alignment(AlignmentX X, AlignmentY Y);
    public record WindowProps(Rect Rect, string Title, Cursor Cursor);
    public record BrushStyle(Color Color);
    public record PenStyle(BrushStyle BrushStyle, double Width);
    public record TextStyle(BrushStyle BrushStyle, string FontFamily, double FontSize, Alignment Alignment);
    public record ShapeStyle(BrushStyle BrushStyle, PenStyle PenStyle);
    public record Line(Point A, Point B);
    public record Ellipse(Point Point, Radius Radius);    
    public record RoundedRect(Rect Rect, Radius Radius);
    public record StyledText(TextStyle Style, string Text);
    public record StyledLine(PenStyle PenStyle, Line Line);
    public record StyledRect(ShapeStyle Style, RoundedRect Rect);
    public record StyledEllipse(ShapeStyle Style, Ellipse Ellipse);
    public record Radius(double X, double Y);

    public interface ICanvas
    {
        ICanvas Draw(StyledText text);
        ICanvas DrawText(StyledLine line);
        ICanvas DrawEllipse(StyledEllipse ellipse);
        ICanvas DrawRect(StyledRect rect);
        Size MeasureText(StyledText text);

        ICanvas SetRect(Rect rect);
        ICanvas PopRect();
    }
}