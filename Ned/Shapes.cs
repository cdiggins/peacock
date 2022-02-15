namespace Ned;

public class Shapes
{
    public Styles Styles { get; }
    public Shapes(Styles styles)  => Styles = styles;

    public StyledRect StyledShape(NodeView view) => new(Styles.ShapeStyle(view), Shape(view));
    public StyledRect StyledShape(SlotView view) => new(Styles.ShapeStyle(view), Shape(view));
    public StyledEllipse StyledShape(SocketView view) => new(Styles.ShapeStyle(view), Shape(view));
    public StyledText StyledText(NodeView view) => new(Styles.HeaderText(view), view.HeaderRect, view.Node.Label);
    public StyledText StyledText(SlotView view) => new(Styles.SlotText(view), view.Rect.ShrinkAndOffset(Dimensions.SlotTextOffset), view.Slot.Label);
    public StyledLine StyledLine(ConnectionView view) => new(Styles.ConnectionPen(view), view.Line);
    public RoundedRect Shape(NodeView view) => new(view.Rect, Dimensions.NodeRadius);
    public RoundedRect Shape(SlotView view) => new(view.Rect, Dimensions.SlotRadius);
    public Ellipse Shape(SocketView view) => new(view.Point, Dimensions.SocketRadius);    
}

