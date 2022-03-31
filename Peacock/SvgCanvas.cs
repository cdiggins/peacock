using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Peacock;

[Mutable]
public class SvgCanvas : ICanvas
{
    public StringBuilder Text = new StringBuilder();

    public override string ToString()
    {
        return Text + "</svg>";
    }

    public SvgCanvas(int width, int height)
    {
        Text.AppendLine($"<svg width='{width}' height='{height}'>")
    }

    public ICanvas Draw(StyledText text)
    {
        throw new NotImplementedException();
    }

    public ICanvas Draw(StyledLine line)
    {
        throw new NotImplementedException();
    }

    public ICanvas Draw(StyledEllipse ellipse)
    {

    }

    public string SvgColor(Color color)
        => $"";

    public string SvgStyle(ShapeStyle style)
    {
        return "{fill: blue; stroke: pink; stroke - width:5; fill - opacity:0.1; stroke - opacity:0.9}";
    }

    public string SvgRect(Rect rect)
        => $"x='{rect.Left}' y='{rect.Top}' width='{rect.Width}' height='{rect.Height}'";

    public ICanvas Draw(StyledRect rect)
    {
        Text.AppendLine($"<rect {SvgRect(rect.Rect.Rect)}/>");
        return this;
    }

    public ICanvas Draw(BrushStyle brushStyle, PenStyle penStyle, Geometry geometry)
    {
        throw new NotImplementedException();
    }

    public Size MeasureText(StyledText text)
    {
        throw new NotImplementedException();
    }

    public ICanvas SetRect(Rect rect)
    {
        throw new NotImplementedException();
    }

    public ICanvas PopRect()
    {
        throw new NotImplementedException();
    }
}