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
public record View(IModel? Model) : IView;
public record NodeView(Node Node, NodeStyle Style) : View(Node);
public record ConnectionView(Connection Connection, ConnectionStyle Style) : View(Connection);
public record SlotView(Slot Slot, SlotStyle Style) : View(Slot);
public record SocketView(Socket Socket, SocketStyle Style) : View(Socket);
public record GraphView(Graph Graph, GraphStyle Style) : View(Graph);

public static class StyleExtensions
{
    public static StyledRect StyledShape(this NodeView view) => new(view.Style.ShapeStyle, view.Shape());
    public static StyledRect StyledShape(this SlotView view) => new(view.Style.ShapeStyle, view.Slot.Rect);
    public static StyledEllipse StyledShape(this SocketView view) => new(view.Style.ShapeStyle, Shape(view));
    public static StyledText StyledText(this SlotView view) => new(view.Style.TextStyle, view.Slot.Rect.ShrinkAndOffset(view.TextOffset()), view.Slot.Label);
    public static StyledText StyledTypeText(this SlotView view) => new(view.Style.SmallTextStyle, view.Slot.Rect.Shrink(view.TextOffset()), view.Slot.Type);
    public static RoundedRect Shape(this NodeView view) => new(view.Node.Rect, view.Style.Radius);
    public static RoundedRect Shape(this SlotView view) => new(view.Slot.Rect, view.Style.Radius);
    public static Ellipse Shape(this SocketView view) => new(view.Socket.Rect.Center(), view.Style.Radius);
    public static Size TextOffset(this SlotView view) => new(view.Style.Radius.X * 1.5, 0);
}