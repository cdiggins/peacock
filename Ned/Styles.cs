﻿using System.Windows.Media;

namespace Ned;

public class Styles
{
    public BrushStyle TextBrush => new(Colors.Black);
    public BrushStyle TransparentBrush => new(Colors.Transparent);
    public PenStyle NodePen(NodeView node) => new(new(Colors.DarkBlue), Dimensions.NodeBorder);
    public BrushStyle NodeBrush(NodeView node) => new(Colors.AntiqueWhite);
    public TextStyle HeaderText(NodeView view) => new(TextBrush, "Segoe UI", Dimensions.HeaderTextSize, new(AlignmentX.Center, AlignmentY.Center));    
    public PenStyle SlotPen(SlotView view) => new(new(Colors.Silver), 0.5);
    public TextStyle SlotText(SlotView view) => new(TextBrush, "Segoe UI", Dimensions.SlotTextSize, new(AlignmentX.Left, AlignmentY.Center));
    public BrushStyle SlotBrush(SlotView view) => TransparentBrush;
    public PenStyle SocketPen(SocketView view) => new(new(Colors.DarkBlue), Dimensions.SocketBorder);
    public BrushStyle SocketBrush(SocketView view) => new(Colors.AntiqueWhite);
    public ShapeStyle ShapeStyle(SocketView view) => new(SocketBrush(view), SocketPen(view));
    public ShapeStyle ShapeStyle(SlotView view) => new(SlotBrush(view), SlotPen(view));
    public ShapeStyle ShapeStyle(NodeView view) => new(NodeBrush(view), NodePen(view));
    public BrushStyle ConnectionBrush(ConnectionView view) => new(Colors.Maroon);
    public PenStyle ConnectionPen(ConnectionView view) => new(ConnectionBrush(view), Dimensions.ConnectionWidth);
}

