using System.Windows;
using System.Windows.Media;
using Peacock;

namespace Emu;

// Styles 
public record SlotStyle(ShapeStyle ShapeStyle, TextStyle TextStyle, TextStyle SmallTextStyle, Radius Radius);
public record SocketStyle(ShapeStyle ShapeStyle, TextStyle TextStyle, Radius Radius);
public record NodeStyle(ShapeStyle ShapeStyle, TextStyle TextStyle, Radius Radius, Color ShadowColor);
public record ConnectionStyle(ShapeStyle ShapeStyle, TextStyle TextStyle);
public record GraphStyle(ShapeStyle ShapeStyle, TextStyle TextStyle);

// Views
public record View(IModel? Model, Rect Rect) : IView;
public record NodeView(Node Node, Rect Rect, NodeStyle Style) : View(Node, Rect);
public record ConnectionView(Connection? Connection, Rect Rect, Line Line, ConnectionStyle Style) : View(Connection, Rect);
public record SlotView(Slot Slot, Rect Rect, SlotStyle Style) : View(Slot, Rect);
public record SocketView(Socket Socket, Rect Rect, Point Point, SocketStyle Style) : View(Socket, Rect);
public record GraphView(Graph Graph, Rect Rect, GraphStyle Style) : View(Graph, Rect);

public static class StyleExtensions
{
    public static StyledRect StyledShape(this NodeView view) => new(view.Style.ShapeStyle, new(view.Rect));
    public static StyledRect StyledShape(this SlotView view) => new(view.Style.ShapeStyle, new(view.Rect));
    public static StyledEllipse StyledShape(this SocketView view) => new(view.Style.ShapeStyle, Shape(view));
    public static StyledText StyledText(this SlotView view) => new(view.Style.TextStyle, view.Rect.ShrinkAndOffset(view.TextOffset()), view.Slot.Label);
    public static StyledText StyledTypeText(this SlotView view) => new(view.Style.SmallTextStyle, view.Rect.Shrink(view.TextOffset()), view.Slot.Type);
    public static StyledLine StyledLine(this ConnectionView view) => new(view.Style.ShapeStyle.PenStyle, view.Line);
    public static RoundedRect Shape(this NodeView view) => new(view.Rect, view.Style.Radius);
    public static RoundedRect Shape(this SlotView view) => new(view.Rect, view.Style.Radius);
    public static Ellipse Shape(this SocketView view) => new(view.Point, view.Style.Radius);
    public static Size TextOffset(this SlotView view) => new(view.Style.Radius.X * 1.5, 0);
}