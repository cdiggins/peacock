using System;
using System.Windows;
using System.Windows.Media;

namespace Ned;

public static class ViewDrawing
{
    // TODO: we can't have this as a static if we want to have dynamic theming etc. 
    public static Shapes Shapes => new(new());

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
            .Draw(Shapes.NodeShadow(view))
            .Draw(Shapes.StyledShape(view));
    
    public static ICanvas DrawHeader(this ICanvas canvas, HeaderView view)
        => canvas.Draw(Shapes.StyledShape(view)).Draw(Shapes.StyledText(view));

    public static ICanvas DrawSlot(this ICanvas canvas, SlotView view)
        => canvas
            .Draw(Shapes.StyledShape(view))
            .Draw(Shapes.StyledText(view))
            .Draw(Shapes.StyledTypeText(view));

    public static ICanvas DrawSocket(this ICanvas canvas, SocketView view)
        => view.Socket.Type == "Array" 
            ? canvas.Draw(Shapes.StyledShapeArraySocket(view))
            : canvas.Draw(Shapes.StyledShape(view));

    public static ICanvas DrawConnection(this ICanvas canvas, ConnectionView view)
        => canvas.DrawConnection(view.Line.A, view.Line.B, Colors.Crimson);
}

