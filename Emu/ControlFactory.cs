using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

    public static Color SocketPenColor(SocketView view)
        => view.Socket.Type switch
        {
            "Any" => Colors.ForestGreen,
            "Number" => Colors.Magenta,
            "Decimal" => Colors.Orange,
            "Array" => Colors.DodgerBlue,
            "AbsoluteCenter 2D" => Colors.Firebrick,
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
    public static SocketStyle DefaultSocketStyle => new(DefaultShapeStyle, DefaultSocketTextStyle, 6, 8); 
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
        => new(new(graph, GraphStyle),
            graph.Nodes.Select(Create).ToList(),
            graph.Connections.Select(Create).ToList(),
            UpdateModel);

    public static Measures NodeMeasures(Node node)
        => new(new Point(), node.Rect);

    public static double HeaderHeight(Node node)
        => SlotHeight(node);

    public static double SlotHeight(Node node)
        => node.Rect.Height / ((double)node.Slots.Count + 1);

    public static Measures HeaderMeasures(Node node)
        => NodeMeasures(node).Relative(new Size(node.Rect.Width, HeaderHeight(node)));

    public static Rect SlotRect(Node node, int i)
        => new(new(0, HeaderHeight(node) + SlotHeight(node) * i),
            new Size(node.Rect.Width, SlotHeight(node)));

    public static Measures SlotMeasures(Node node, int i)
        => NodeMeasures(node).Relative(SlotRect(node, i));

    public Rect SocketRect(Point point) 
        => new(point.X - SlotRadius, point.Y - SlotRadius, SlotRadius * 2, SlotRadius * 2);
    
    public Measures SocketMeasures(Socket socket, Measures slotMeasures)
        => socket.LeftOrRight
            ? slotMeasures.Relative(SocketRect(slotMeasures.ClientRect.LeftCenter())) 
            : slotMeasures.Relative(SocketRect(slotMeasures.ClientRect.RightCenter()));

    public NodeControl Create(Node node)
        => new(
            NodeMeasures(node),
            new(node, NodeStyle),
            Create(node.Header, HeaderMeasures(node)),
            node.Slots.Select((slot, i) => Create(slot, SlotMeasures(node, i))).ToList(), 
            UpdateModel);

    public SlotControl Create(Slot slot, Measures slotMeasures)
        => new(slotMeasures,
            new(slot, slot.IsHeader ? HeaderStyle : SlotStyle),
            Create(slot.Left, slotMeasures),
            Create(slot.Right, slotMeasures), UpdateModel);

    public SocketControl? Create(Socket? socket, Measures slotMeasures)
        => socket == null 
            ? null 
            : new(SocketMeasures(socket, slotMeasures), new(socket, SocketStyle), UpdateModel);

    public ConnectionControl Create(Connection conn)
        => new(new(conn, ConnectionStyle), UpdateModel);

    public IEnumerable<IControl> Create(IModel model)
        => new[] { Create((Graph)model) };

}