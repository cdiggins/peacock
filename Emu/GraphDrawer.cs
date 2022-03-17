using System;
using System.Windows;
using System.Windows.Media;
using Peacock;

namespace Emu;

public static class ViewDrawing
{
    public static Geometry ConnectorGeometry(Point a, Point b)
    {
        var xDelta = Math.Abs(a.X - b.X);
        var xDist = Math.Clamp(xDelta * 0.4, 75, double.MaxValue);
        var controlPointA = a.Add(new Point(xDist, 0));
        var controlPointB = b.Subtract(new Point(xDist, 0));
        var segment1 = new BezierSegment
        {
            Point1 = controlPointA,
            Point2 = controlPointB,
            Point3 = b
        };
        var pathFigure = new PathFigure(a, new[] { segment1 }, false);
        var pathGeometry = new PathGeometry(new [] { pathFigure });
        return pathGeometry;
    }

    public static ICanvas DrawConnection(this ICanvas canvas, Point start, Point end, Color color)
        => canvas.Draw(new(Colors.Transparent), new(new(color), 5), ConnectorGeometry(start, end));

    public static ICanvas DrawGraph(this ICanvas canvas, GraphView view)
        => canvas;

    public static ICanvas DrawNode(this ICanvas canvas, NodeView view)
        => canvas
            .Draw(NodeShadow(view))
            .Draw(StyledShape(view));
    
    public static ICanvas DrawHeaderSlot(this ICanvas canvas, SlotView view)
        => canvas
            .Draw(StyledShape(view))
            .Draw(StyledText(view));

    public static ICanvas DrawSlot(this ICanvas canvas, SlotView view)
        => view.Slot.IsHeader 
            ? DrawHeaderSlot(canvas, view) 
            : DrawNonHeaderSlot(canvas, view);

    public static ICanvas DrawNonHeaderSlot(this ICanvas canvas, SlotView view)
        => canvas
            .Draw(StyledShape(view))
            .Draw(StyledText(view))
            .Draw(StyledTypeText(view));

    public static ICanvas DrawSocket(this ICanvas canvas, SocketView view)
        => view.Socket.Type == "Array" 
            ? canvas.Draw(StyledShapeArraySocket(view))
            : canvas.Draw(StyledShape(view));

    public static ICanvas DrawConnection(this ICanvas canvas, ConnectionView view)
        => canvas.DrawConnection(view.Line.A, view.Line.B, view.Style.ShapeStyle.Color);
}