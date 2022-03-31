using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Peacock;

namespace Emu;

public record GraphControl(GraphView View)
    : Control<GraphView>(View)
{
    public override IEnumerable<IControl> GetChildren(IControlFactory factory)
        => View.Graph.Nodes.Select(node => factory.Create(this, node))
            .Concat(View.Graph.Connections.Select(conn => factory.Create(this, conn)));
}

public record SocketControl(SocketView View) 
    : Control<SocketView>(View)
{
    public override ICanvas Draw(ICanvas canvas) 
        => canvas.Draw(View.StyledShape());
}

public record SlotControl(SlotView View) 
    : Control<SlotView>(View)
{
    public override ICanvas Draw(ICanvas canvas) 
        => View.Slot.IsHeader 
            ? canvas         
                .Draw(View.StyledShape())
                .Draw(View.StyledText())
            : canvas
                .Draw(View.StyledShape())
                .Draw(View.StyledText())
                .Draw(View.StyledTypeText());

    public override IEnumerable<IControl> GetChildren(IControlFactory factory)
    {
        if (View.Slot.Left != null)
            yield return factory.Create(this, View.Slot.Left);

        if (View.Slot.Right != null)
            yield return factory.Create(this, View.Slot.Right);
    }
}

public record NodeControl(NodeView View) 
    : Control<NodeView>(View)
{
    // TODO: more of this should be in the View 
    public StyledEllipse NodeShadow() => new(
        new ShapeStyle(View.Style.ShadowColor, PenStyle.Empty),
        new Ellipse(View.Node.Rect.BottomCenter(), new Radius(View.Node.Rect.HalfWidth() * 1.3, View.Node.Rect.HalfWidth() * 0.3)));

    public override ICanvas Draw(ICanvas canvas)
        => canvas.Draw(NodeShadow())
            .Draw(View.StyledShape());

    public override IEnumerable<IControl> GetChildren(IControlFactory factory)
        => View.Node.Slots.Select(slot => factory.Create(this, slot))
            .Prepend(factory.Create(this, View.Node.Header));
}

public record ConnectionControl(ConnectionView View)
    : Control<ConnectionView>(View)
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