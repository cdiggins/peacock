using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Emu.Behaviors;
using Peacock;

namespace Emu.Controls;

public record GraphStyle(ShapeStyle ShapeStyle, TextStyle TextStyle, double GridDistance);

public record GraphView(Graph Graph, GraphStyle Style) : View(Graph, Graph.Id);

public record GraphControl(GraphView View,
    IReadOnlyList<NodeControl> Nodes,
    Func<IUpdates, IControl, IControl, IUpdates> Callback)
    : Control<GraphView>(Measures.Default, View, Nodes, Callback)
{
    public override IEnumerable<IBehavior> GetDefaultBehaviors()
        => new[] { new ConnectingBehavior(this) };

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

    // TODO: get the style from the style class. 
    public ICanvas DrawConnector(ICanvas canvas, Point a, Point b)
        => canvas.Draw(new(Colors.Transparent), new(Colors.Blue, 4),
                ConnectorGeometry(a, b));

    public override ICanvas Draw(ICanvas canvas)
    {
        var gridDistance = View.Style.GridDistance;

        var client = new Rect(0, 0, 2000, 2000);

        // Fill the background
        canvas = canvas.Draw(new StyledRect(new ShapeStyle(Colors.Black, PenStyle.Empty), client));

        // Draw vertical lines 
        for (var i = gridDistance; i < client.Width; i += gridDistance)
        {
            canvas = canvas.Draw(new StyledLine(View.Style.ShapeStyle.PenStyle, new Line(new Point(i, 0), new Point(i, client.Height))));
        }

        // Draw horizontal lines 
        for (var i = gridDistance; i < client.Height; i += gridDistance)
        {
            canvas = canvas.Draw(new StyledLine(View.Style.ShapeStyle.PenStyle, new Line(new Point(0, i), new Point(client.Width, i))));
        }

        var socketPoints = this.GetSockets().ToDictionary(s => s.View.Model.Id, s => s.AbsoluteCenter());
        foreach (var c in View.Graph.Connections)
        {
            var p1 = socketPoints[c.SourceId];
            var p2 = socketPoints[c.DestinationId];
            canvas = DrawConnector(canvas, p1, p2);
        }
        return base.Draw(canvas);
    }
}