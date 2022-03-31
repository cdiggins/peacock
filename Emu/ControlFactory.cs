using System;
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

    public IControl Create(IControl? parent, IModel model)
        => model switch
        {
            Graph g 
                => new GraphControl(new(g, GraphStyle)),
            
            Node n  
                => new NodeControl(new(n, NodeStyle)),
            
            Slot s 
                => s.IsHeader 
                    ? new SlotControl(new(s, HeaderStyle))
                    : new SlotControl(new(s, SlotStyle)),
            
            Connection c 
                => new ConnectionControl(new(c, ConnectionStyle)),

            Socket k 
                => new SocketControl(new(k, SocketStyle)),

            _ 
                => throw new NotImplementedException("Unrecognized model")
        };
}