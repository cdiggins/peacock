using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Peacock;

namespace Emu;

public record GraphControl(Rect Rect, GraphView View, Func<IUpdates, IControl, IControl, IUpdates> Callback)
    : Control<GraphView>(Rect, View, Callback)
{
    public override IEnumerable<IControl> GetChildren(IControlFactory factory)
        => View.Graph.Nodes.SelectMany(factory.Create)
            .Concat(View.Graph.Connections.SelectMany(factory.Create));
}

public record SocketControl(SocketView View, Func<IUpdates, IControl, IControl, IUpdates> Callback) 
    : Control<SocketView>(Rect.Empty, View, Callback)
{
    public override ICanvas Draw(ICanvas canvas) 
        => canvas.Draw(View.StyledShape());
}

public record SlotControl(SlotView View, Func<IUpdates, IControl, IControl, IUpdates> Callback) 
    : Control<SlotView>(Rect.Empty, View, Callback)
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
            foreach (var c in factory.Create(View.Slot.Left))
                yield return c;

        if (View.Slot.Right != null)
            foreach (var c in factory.Create(View.Slot.Right))
                yield return c;
    }
}

public record NodeControl(NodeView View, Func<IUpdates, IControl, IControl, IUpdates> Callback) 
    : Control<NodeView>(View.Node.Rect, View, Callback)
{
    // TODO: more of this should be in the View 
    public StyledEllipse NodeShadow() => new(
        new ShapeStyle(View.Style.ShadowColor, PenStyle.Empty),
        new Ellipse(View.Node.Rect.BottomCenter(), new Radius(View.Node.Rect.HalfWidth() * 1.3, View.Node.Rect.HalfWidth() * 0.3)));

    public override ICanvas Draw(ICanvas canvas)
        => canvas.Draw(NodeShadow())
            .Draw(View.StyledShape());

    public override IEnumerable<IControl> GetChildren(IControlFactory factory)
        => factory.Create(View.Node.Header).Concat(View.Node.Slots.SelectMany(factory.Create));

    public override IEnumerable<IBehavior> GetDefaultBehaviors()
        => new[] { new DraggingBehavior(this) };
}

public record ConnectionControl(ConnectionView View, Func<IUpdates, IControl, IControl, IUpdates> Callback)
    : Control<ConnectionView>(Rect.Empty, View, Callback)
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