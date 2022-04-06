using System;
using System.Windows;
using System.Windows.Media;
using Peacock;

namespace Emu.Controls;

public record ConnectionStyle(ShapeStyle ShapeStyle, TextStyle TextStyle);

public record ConnectionView(Connection Connection, ConnectionStyle Style) : View(Connection, Connection.Id);

public record ConnectionControl(ConnectionView View, Func<IUpdates, IControl, IControl, IUpdates> Callback)
    : Control<ConnectionView>(Measures.Default, View, Callback)
{
    public Geometry ConnectorGeometry()
        => ConnectorGeometry(View.Connection.Line.A, View.Connection.Line.B);

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
        var pathGeometry = new PathGeometry(new[] { pathFigure });
        return pathGeometry;
    }

    public override ICanvas Draw(ICanvas canvas) 
        => canvas.Draw(new(Colors.Transparent), View.Style.ShapeStyle.PenStyle, ConnectorGeometry());
}