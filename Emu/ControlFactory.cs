using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Emu.Controls;
using Peacock;

namespace Emu;

public record ControlFactory : IControlFactory
{
    public int NodeHeaderHeight { get; init; } = 25;
    public int NodeSlotHeight { get; init; } = 20;
    public int NodeWidth { get; init; } = 110;
    public int SlotRadius { get; init; } = 5;

    public double GetNodeHeight(int slots) => NodeHeaderHeight + slots * NodeSlotHeight;
    public Rect GetNodeHeaderRect(Rect nodeRect) => new(new Point(), new Size(nodeRect.Width, NodeHeaderHeight));
    public Rect GetSocketRect(Rect slotRect, bool leftOrRight) => GetSocketRect(leftOrRight ? slotRect.LeftCenter() : slotRect.RightCenter());
    public Rect GetSocketRect(Point point) => new(point.X - SlotRadius, point.Y - SlotRadius, SlotRadius * 2, SlotRadius * 2);
    public Rect GetSlotRect(Rect rect, int i) => new(0, NodeHeaderHeight + i * NodeSlotHeight, rect.Width, NodeSlotHeight);

    public static Color SocketPenColor(SocketView view)
        => view.Socket.Type switch
        {
            "Any" => Colors.ForestGreen,
            "Number" => Colors.Magenta,
            "Decimal" => Colors.Orange,
            "Array" => Colors.DodgerBlue,
            "Center 2D" => Colors.Firebrick,
            "Size" => Colors.Firebrick,
            _ => Colors.DarkBlue,
        };

    public static Color GetColor(NodeKind kind)
        => kind switch
        {
            NodeKind.PropertySet => Colors.LightSeaGreen,
            NodeKind.OperatorSet => Colors.DodgerBlue,
            NodeKind.Input => Colors.Chocolate,
            NodeKind.Output => Colors.Orange,
            _ => Colors.Gray
        };

    public static TextStyle DefaultTextStyle = new(Colors.Black, "Segoe UI", 12, new(AlignmentX.Left, AlignmentY.Center));

    public static ShapeStyle DefaultShapeStyle = new(Colors.DarkSeaGreen, new(Colors.DarkOrange, 0.5));

    public static TextStyle DefaultHeaderTextStyle => DefaultTextStyle with { FontSize = 14, Alignment = Alignment.CenterCenter };
    public static TextStyle DefaultSlotTextStyle => DefaultTextStyle with { FontSize = 12, Alignment = Alignment.LeftCenter };
    public static TextStyle DefaultSlotTypeTextStyle => DefaultTextStyle with { FontSize = 8, Alignment = Alignment.RightTop };
    public static TextStyle DefaultSocketTextStyle => DefaultTextStyle with { FontSize = 6, Alignment = Alignment.RightTop };

    public static NodeStyle DefaultNodeStyle => new(DefaultShapeStyle, DefaultTextStyle, 0, Color.FromArgb(0x66, 0x33, 0x33, 0x33));
    public static SocketStyle DefaultSocketStyle => new(DefaultShapeStyle, DefaultSocketTextStyle, 6); 
    public static SlotStyle DefaultSlotStyle => new(DefaultShapeStyle, DefaultSlotTextStyle, DefaultSlotTypeTextStyle, 8);
    public static SlotStyle DefaultHeaderStyle => new(DefaultShapeStyle, DefaultHeaderTextStyle, DefaultSlotTypeTextStyle, 8);

    public SocketStyle SocketStyle { get; init; } = DefaultSocketStyle;
    public SlotStyle SlotStyle { get; init; } = DefaultSlotStyle;
    public SlotStyle HeaderStyle { get; init; } = DefaultHeaderStyle;
    public NodeStyle NodeStyle { get; init; } = DefaultNodeStyle;

    public ConnectionStyle ConnectionStyle { get; init; } = new(DefaultShapeStyle, DefaultTextStyle);
    
    public GraphStyle GraphStyle { get; init; } = new(DefaultShapeStyle, DefaultTextStyle);

    public IUpdates UpdateModel(IUpdates updates, IControl oldControl, IControl newControl)
        => newControl switch
        {
            GraphControl gc 
                => updates,

            NodeControl nc
                // TODO: So this will probably work, but it seems a bit heavy-handed. 
                // The current model is just obliterated with the one in the newControl. 
                // There is no way to apply incremental updates. Each newControl just replaces  
                // any other model change that another previously did. 
                // What I really want is to compute the delta from the previous to the next,
                // and that this function is responsible for applying the delta.
                => updates.UpdateModel(nc.View.Node, model => nc.View.Node),

            SlotControl sc
                => updates,

            ConnectionControl cc
                => updates,

            SocketControl sc 
                => updates,

            _
                => throw new NotImplementedException($"Unrecognized newControl {oldControl}")
        };

    public GraphControl Create(Graph graph)
        => new(Rect.Empty, new(graph, GraphStyle),
            graph.Nodes.Select(Create).ToList(),
            graph.Connections.Select(Create).ToList(),
            UpdateModel);

    public Rect HeaderRect(Node node)
        => new(node.Rect.TopLeft, new Size(node.Rect.Width, NodeHeaderHeight));

    public NodeControl Create(Node node)
        => new(
            new(node, NodeStyle), 
            Create(node.Header, HeaderRect(node)), 
            node.Slots.Select((slot, i) => Create(slot, GetSlotRect(node.Rect, i))).ToList(), 
            UpdateModel);

    public SlotControl Create(Slot slot, Rect rect)
        => slot.IsHeader
            ? new (rect, new(slot, HeaderStyle), Create(slot.Left, rect), Create(slot.Right, rect), UpdateModel)
            : new (rect, new(slot, SlotStyle), Create(slot.Left, rect), Create(slot.Right, rect), UpdateModel);

    public SocketControl? Create(Socket? socket, Rect slotRect)
        => socket == null 
            ? null 
            : new(GetSocketRect(slotRect, socket.LeftOrRight), new(socket, SocketStyle), UpdateModel);

    public ConnectionControl Create(Connection conn)
        => new(new(conn, ConnectionStyle), UpdateModel);

    public IEnumerable<IControl> Create(IModel model)
        => new[] { Create((Graph)model) };

}