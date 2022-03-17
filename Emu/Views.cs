using System.Windows;
using Peacock;

namespace Emu;

public record View(Object? Element) : Object, IView;
public record DimensionedView(Object Object, Rect Rect) : View(Object);
public record NodeView(Node Node, Rect Rect, NodeStyle Style) : DimensionedView(Node, Rect);
public record ConnectionView(Connection? Connection, Line Line, ConnectionStyle Style) : View(Connection);
public record SlotView(Slot Slot, Rect Rect, SlotStyle Style) : DimensionedView(Slot, Rect);
public record SocketView(Socket Socket, Point Point, SocketStyle Style) : View(Socket);
public record GraphView(Graph Graph, GraphStyle Style) : Object, IView;