﻿using System.Windows;
using System.Windows.Media;

namespace Ned;

public class Shapes
{
    public Styles Styles { get; }
    public Shapes(Styles styles)  => Styles = styles;

    public StyledRect StyledShape(NodeView view) => new(Styles.ShapeStyle(view), Shape(view));
    public StyledRect StyledShape(SlotView view) => new(Styles.ShapeStyle(view), Shape(view));
    public StyledRect StyledShape(HeaderView view) => new(Styles.ShapeStyle(view), Shape(view));
    public StyledEllipse StyledShape(SocketView view) => new(Styles.ShapeStyle(view), Shape(view));
    public StyledRect StyledShapeArraySocket(SocketView view) => new(Styles.ShapeStyle(view),
        new RoundedRect(
            new Rect(
                view.Point.Subtract(new Point(Dimensions.SocketRadius.X, Dimensions.SocketRadius.Y)),
                new Size(Dimensions.SocketRadius.X * 2, Dimensions.SocketRadius.Y * 2)),
            new Radius(3, 3)));

    public StyledText StyledText(HeaderView view) => new(Styles.HeaderText(view), view.Rect, view.Header.Label);
    public StyledText StyledText(SlotView view) => new(Styles.SlotText(view), view.Rect.ShrinkAndOffset(Dimensions.SlotTextOffset), view.Slot.Label);
    public StyledText StyledTypeText(SlotView view) => new(Styles.SlotTypeText(view), view.Rect.Shrink(Dimensions.SlotTextOffset), view.Slot.Type);
    public StyledLine StyledLine(ConnectionView view) => new(Styles.ConnectionPen(view), view.Line);
    public RoundedRect Shape(NodeView view) => new(view.Rect, Dimensions.NodeRadius);
    public RoundedRect Shape(SlotView view) => new(view.Rect, Dimensions.SlotRadius);
    public RoundedRect Shape(HeaderView view) => new(view.Rect, Dimensions.NodeRadius);
    public Ellipse Shape(SocketView view) => new(view.Point, Dimensions.SocketRadius);

    public StyledEllipse NodeShadow(NodeView view) => new(
        new ShapeStyle(new BrushStyle(Color.FromArgb(0x66, 0x33, 0x33, 0x33)), Styles.TransparentPen),
        new Ellipse(view.Rect.BottomCenter(), new Radius(view.Rect.HalfWidth() * 1.3, view.Rect.HalfWidth() * 0.3)));
}

