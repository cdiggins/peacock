using System.Windows.Media;
using Peacock;

namespace Bohr;

public class Styles
{
    public BrushStyle TextBrush => new(Colors.Black);
    public BrushStyle TransparentBrush => new(Colors.Transparent);
    public PenStyle TransparentPen => new(TransparentBrush, 0);
    public PenStyle NodePen(NodeView view) => new(new(Colors.DarkBlue), Dimensions.NodeBorder);
    public BrushStyle NodeBrush(NodeView view) => new(Colors.AntiqueWhite);
    public Color Color(NodeKind kind)
        => kind switch
        {
            NodeKind.PropertySet => Colors.LightSeaGreen,
            NodeKind.OperatorSet => Colors.DodgerBlue,
            NodeKind.Input => Colors.Chocolate, 
            NodeKind.Output => Colors.Orange,
            _ => Colors.Gray
        };
    public BrushStyle HeaderBrush(HeaderView view) => new(Color(view.Kind));
    public TextStyle HeaderText(HeaderView view) => new(TextBrush, "Segoe UI", Dimensions.HeaderTextSize, new(AlignmentX.Center, AlignmentY.Center));    
    public PenStyle SlotPen(SlotView view) => new(new(Colors.Silver), 0.5);
    public TextStyle SlotText(SlotView view) => new(TextBrush, "Segoe UI", Dimensions.SlotTextSize, new(AlignmentX.Left, AlignmentY.Center));
    public TextStyle SlotTypeText(SlotView view) => new(TextBrush, "Segoe UI", Dimensions.SlotTypeTextSize, new(AlignmentX.Right, AlignmentY.Top));
    public BrushStyle SlotBrush(SlotView view) => TransparentBrush;

    public Color SocketPenColor(SocketView view)
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

    public PenStyle SocketPen(SocketView view) => new(new(SocketPenColor(view)), Dimensions.SocketBorder);

    public BrushStyle SocketBrush(SocketView view) => new(Colors.AntiqueWhite);
    public ShapeStyle ShapeStyle(SocketView view) => new(SocketBrush(view), SocketPen(view));
    public ShapeStyle ShapeStyle(SlotView view) => new(SlotBrush(view), SlotPen(view));
    public ShapeStyle ShapeStyle(NodeView view) => new(NodeBrush(view), NodePen(view));
    public ShapeStyle ShapeStyle(HeaderView view) => new(HeaderBrush(view), TransparentPen);
    public BrushStyle ConnectionBrush(ConnectionView view) => new(Colors.Maroon);
    public PenStyle ConnectionPen(ConnectionView view) => new(ConnectionBrush(view), Dimensions.ConnectionWidth);
}

