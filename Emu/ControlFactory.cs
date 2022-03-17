using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Peacock;

namespace Emu;

public record ControlFactory : IControlFactory
{
    public static Color SocketPenColor(SocketView view)
        => view.Socket.Type switch
        {
            "Any" => Colors.ForestGreen,
            "Number" => Colors.Magenta,
            "Decimal" => Colors.Orange,
            "Array" => Colors.DodgerBlue,
            "Point 2D" => Colors.Firebrick,
            "Size" => Colors.Firebrick,
            _ => Colors.DarkBlue,
        };

    public static Color Color(NodeKind kind)
        => kind switch
        {
            NodeKind.PropertySet => Colors.LightSeaGreen,
            NodeKind.OperatorSet => Colors.DodgerBlue,
            NodeKind.Input => Colors.Chocolate,
            NodeKind.Output => Colors.Orange,
            _ => Colors.Gray
        };

    public static double NodeWidth => 110;
    public static double NodeSpacing => 20;
    public static double NodeSlotHeight => 20;
    public static double NodeHeaderHeight => 25;
    public static Radius SocketRadius => new(6, 6);
    public static Radius SlotRadius => NodeRadius;
    public static Radius NodeRadius => new(8, 8);
    public static double GetNodeHeight(int slots) => NodeHeaderHeight + slots * NodeSlotHeight;
    public static double NodeBorder => 3;
    public static double ConnectionWidth => 5;
    public static double SocketBorder => 1;
    public static Size SlotTextOffset => new(SocketRadius.X * 1.5, 0);

    public static TextStyle DefaultTextStyle = new(Colors.Black, "Segoe UI", 12, new(AlignmentX.Left, AlignmentY.Center));
    public static ShapeStyle DefaultShapeStyle = new(Colors.DarkSeaGreen, Colors.DarkOrange, 0.5, new(3, 3));

    public static Rect SocketRect => new(new(SocketRadius.X, SocketRadius.Y));
    public static Rect SlotRect => new(new(NodeWidth, NodeSlotHeight));
    public static Rect HeaderRect => new(new(NodeWidth, NodeHeaderHeight));
    public static Rect NodeRect => new(new(NodeWidth, NodeHeaderHeight));

    public static SocketStyle DefaultSocketStyle => new(DefaultShapeStyle, DefaultTextStyle, SocketRect);
    public static SlotStyle DefaultSlotStyle => new(DefaultShapeStyle, DefaultSlotTextStyle, DefaultSlotTypeTextStyle, SlotRect);
    public static SlotStyle DefaultHeaderStyle => new(DefaultShapeStyle, DefaultHeaderTextStyle, DefaultSlotTypeTextStyle, HeaderRect);
    public static NodeStyle DefaultNodeStyle => new(DefaultShapeStyle, DefaultTextStyle, NodeRect);
    public static ConnectionStyle DefaultConnectionStyle => new(DefaultShapeStyle, DefaultTextStyle);
    public static GraphStyle DefaultGraphStyle => new(DefaultShapeStyle, DefaultTextStyle);

    public static TextStyle DefaultHeaderTextStyle => DefaultTextStyle with { FontSize = 14, Alignment = Alignment.CenterCenter };
    public static TextStyle DefaultSlotTextStyle => DefaultTextStyle with { FontSize = 12, Alignment = Alignment.LeftCenter };
    public static TextStyle DefaultSlotTypeTextStyle =>  DefaultTextStyle with { FontSize = 8, Alignment = Alignment.RightTop };

    public SocketStyle SocketStyle { get; init; } = DefaultSocketStyle;
    public SlotStyle SlotStyle { get; init; } = DefaultSlotStyle;
    public SlotStyle HeaderStyle { get; init; } = DefaultHeaderStyle;
    public NodeStyle NodeStyle { get; init; } = DefaultNodeStyle;
    public ConnectionStyle ConnectionStyle { get; init; } = DefaultConnectionStyle;
    public GraphStyle GraphStyle { get; init; } = DefaultGraphStyle;

    public Rect GetNodeRect(Node node)
        => new(0, 0, NodeWidth, GetNodeHeight(node.Slots.Count));

    public IControl CreateControl(IControl parent, IObject model)
        => model switch
        {
            Graph g 
                => new GraphControl(new(g, GraphStyle)),
            Node n 
                => new NodeControl(new(n, GetNodeRect(n), NodeStyle)),
            Slot s 
                => s.IsHeader 
                    ? new SlotControl(new(s, SlotRect, SlotStyle))
                    : new SlotControl(new(s, SlotRect, HeaderStyle)),
            Connection c 
                => new ConnectionControl(new(c, new(new(),new()), ConnectionStyle)),
            Socket k 
                => new SocketControl(new(k, new(), SocketStyle)),
            _ 
                => throw new NotImplementedException("Unrecognized model")
        };

    public Dictionary<Guid, Point> GetSocketPositions(ControlManager mgr, IEnumerable<NodeControl> controls)
        => GetSockets(mgr, controls).ToDictionary(s => s.Id, s => s.View.Point);

    public IEnumerable<SocketControl> GetSockets(ControlManager manager, IEnumerable<IControl> controls)
        => controls.SelectMany(c => GetSockets(manager, controls));

    public IEnumerable<SocketControl> GetSockets(ControlManager manager, IControl control)
        => control is SocketControl k 
            ? Enumerable.Repeat(k, 1) 
            : manager.GetChildren(control).SelectMany(x => GetSockets(manager, x));

    public ConnectionControl UpdateConnectionPositions(ConnectionControl control, Point a, Point b)
        => (ConnectionControl)control.With((ConnectionView view) => view with { Line = new Line(a, b) });

    public Point ConnectionSourcePoint(ConnectionControl control, Dictionary<Guid, Point> lookup)
        => control.View.Connection != null ? lookup[control.View.Connection.Source.Id] : control.View.Line.A;

    public Point ConnectionDestPoint(ConnectionControl control, Dictionary<Guid, Point> lookup)
        => control.View.Connection != null ? lookup[control.View.Connection.Destination.Id] : control.View.Line.B;

    public ConnectionControl UpdateConnectionPositions(ConnectionControl control, Dictionary<Guid, Point> lookup)
        => UpdateConnectionPositions(control, ConnectionSourcePoint(control, lookup),ConnectionDestPoint(control, lookup));

    public IEnumerable<ConnectionControl> UpdateConnectionPositions(IEnumerable<ConnectionControl> connectionControls,
        Dictionary<Guid, Point> lookup)
        => connectionControls.Select(c => UpdateConnectionPositions(c, lookup));

    public ControlManager CreateChildren(ControlManager manager, IControl parent)
    {
        // TODO: should this function actually remove the previous children? I think perhaps this should be managed by the infrastructure
        manager = manager.RemoveChildren(parent);
        switch (parent)
        {
            case GraphControl g:
            {
                manager = manager.AddChildren(parent, manager.GetOrCreateControls(parent, g.View.Graph.Nodes));
                var connectionControls = manager.GetOrCreateControls(parent, g.View.Graph.Connections).OfType<ConnectionControl>();
                var socketPositions = GetSocketPositions(manager, manager.AllControls().OfType<NodeControl>());
                connectionControls = UpdateConnectionPositions(connectionControls, socketPositions);
                return manager
                    .AddChildren(parent, connectionControls);
            }
            case NodeControl n:
            {
                return manager
                    .AddChild(parent, manager.GetOrCreateControl(parent, n.View.Node.Header))
                    .AddChildren(parent, manager.GetOrCreateControls(parent, n.View.Node.Slots));
            }
            case SlotControl s:
            {
                return manager
                    .AddChildren(parent, manager.GetOrCreateControls(parent, s.View.Slot.Left, s.View.Slot.Right));
            }
            case ConnectionControl:
            {
                return manager;
            }
            case SocketControl:
            {
                return manager;
            }
        }
        throw new NotImplementedException("Unrecognized control");
    }
}