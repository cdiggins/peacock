using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Media;
using Emu.Controls;
using Peacock;

namespace Emu;

// We like: borders. ANd borders around sockets. We don't love borders around slots.
// WE want themes.
// Flip text on and off.

public record ControlFactory : IControlFactory
{
    public double NodeHeaderMultiplier = 1.5;
    public int NodeSlotHeight { get; init; } = 20;
    public int NodeWidth { get; init; } = 130;
    public int SocketRadius { get; init; } = 5;
    public int GridDistance = 40;

    public Color GetSocketColor(Socket socket)
        => socket.Type switch
        {
            "Any" => Colors.ForestGreen,
            "Number" => Colors.Magenta,
            "Decimal" => Colors.Orange,
            "Array" => Colors.DodgerBlue,
            "Point 2D" => Colors.Firebrick,
            "Size" => Colors.Firebrick,
            _ => Colors.DarkBlue,
        };

    public Color GetNodeColor(NodeKind kind)
        => kind switch
        {
            NodeKind.PropertySet => Colors.Chartreuse,
            NodeKind.OperatorSet => Colors.DeepPink,
            NodeKind.Input => Colors.Yellow,
            NodeKind.Output => Colors.Cyan,
            _ => Colors.White
        };

    public Color BackgroundColor = Colors.Black;
    public Color GridColor = Color.FromRgb(0x33, 0x33, 0x33);

    public string Font => Fonts[3];

    public string[] Fonts => new[]
    {
        "Verdana",
        "Open Sans",
        "Noto",
        "Lato",
        "Roboto",
        "Segoe UI",
        "Trebuchet MS",
        "Barlow",
        "Calibri",
        "Gill Sans Nova"
    };

    public TextStyle TextStyle => new(Colors.WhiteSmoke, Font, 16, new(AlignmentX.Left, AlignmentY.Center));
    public TextStyle SlotTextStyle => TextStyle with { FontSize = 10, Alignment = Alignment.LeftCenter };
    public TextStyle SlotTypeTextStyle => TextStyle with { FontSize = 8, Alignment = Alignment.RightTop };
    public TextStyle SocketTextStyle => TextStyle with { FontSize = 6, Alignment = Alignment.RightTop };

    public GraphStyle GraphStyle
        => new(new(BackgroundColor, GridColor), TextStyle, GridDistance);

    public NodeStyle GetNodeStyle(Node node)
        => new(new(BackgroundColor, GetNodeColor(node.Kind)), TextStyle, 0);

    public SlotStyle GetSlotStyle(Slot slot)
        => new(new(BackgroundColor, Colors.Transparent), SlotTextStyle, SocketTextStyle, 0);

    public SocketStyle GetSocketStyle(Socket socket)
        => new(new(GetSocketColor(socket), GetSocketColor(socket)), SocketTextStyle, SocketRadius, SocketRadius + 2);

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
                => updates.UpdateModel(nc.View.Node, _ => nc.View.Node),

            SlotControl sc
                => updates,

            SocketControl sc 
                => updates,

            _
                => throw new NotImplementedException($"Unrecognized newControl {oldControl}")
        };

    public GraphControl Create(Graph graph)
        => new(new(graph, GraphStyle),
            graph.Nodes.Select(Create).ToList(),
            UpdateModel);

    public Measures NodeMeasures(Node node)
        => new(new Point(), node.Rect);

    public double HeaderHeight(Node node)
        => SlotHeight(node) * NodeHeaderMultiplier;

    public double SlotHeight(Node node)
        => node.Rect.Height / (node.Slots.Count + NodeHeaderMultiplier);

    public Rect SlotRect(Node node, int i)
        => new(new(0, HeaderHeight(node) + SlotHeight(node) * i),
            new Size(node.Rect.Width, SlotHeight(node)));

    public Measures SlotMeasures(Node node, int i)
        => NodeMeasures(node).Relative(SlotRect(node, i));

    public Rect SocketRect(Point point) 
        => new(point.X - SocketRadius, point.Y - SocketRadius, SocketRadius * 2, SocketRadius * 2);
    
    public Measures SocketMeasures(Socket socket, Measures slotMeasures)
        => socket.LeftOrRight
            ? slotMeasures.Relative(SocketRect(slotMeasures.ClientRect.LeftCenter())) 
            : slotMeasures.Relative(SocketRect(slotMeasures.ClientRect.RightCenter()));

    public NodeControl Create(Node node)
        => new(
            NodeMeasures(node),
            new(node, GetNodeStyle(node)),
            node.Slots.Select((slot, i) => Create(slot, SlotMeasures(node, i))).ToList(), 
            UpdateModel);

    public SlotControl Create(Slot slot, Measures slotMeasures)
        => new(slotMeasures,
            new(slot, GetSlotStyle(slot)),
            Create(slot.Left, slotMeasures),
            Create(slot.Right, slotMeasures), UpdateModel);

    public SocketControl? Create(Socket? socket, Measures slotMeasures)
        => socket == null 
            ? null 
            : new(SocketMeasures(socket, slotMeasures), new(socket, GetSocketStyle(socket)), UpdateModel);

    public IEnumerable<IControl> Create(IModel model)
        => new[] { Create((Graph)model) };

}