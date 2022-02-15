using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Ned;

public static class ModelToView
{
    // Given a list of nodes, and a previous (potentially null) list of node views, creates a new set of node views. 
    public static IReadOnlyList<NodeView> ToViews(this IReadOnlyList<Node> nodes, IReadOnlyList<NodeView>? views)
    {
        var nodeLookup = nodes.ToDictionary(n => n.Id, n => n);
        var viewLookup = views?.ToDictionary(v => v.Id, v => v) ?? new Dictionary<Guid, NodeView>();

        var result = new List<NodeView>();
        var cnt = 0;

        var offsetX = 50;
        var offsetY = 50;

        foreach (var node in nodes)
        {
            viewLookup.TryGetValue(node.Id, out var oldView);

            // TODO: change the algorithm to keep putting them to the left of the previous views, right now starts in upper left corner 

            var rect = oldView?.Rect ?? 
                new Rect(new Point(offsetX + cnt++ * (Dimensions.NodeWidth + Dimensions.NodeSpacing), offsetY), new Size(Dimensions.NodeWidth, Dimensions.NodeHeight(node.Slots.Count)));
            var newView = node.ToView(rect, ToViews(node.Slots, rect.TopLeft, oldView?.SlotViews));

            result.Add(newView);
        }

        return result;
    }

    public static IEnumerable<SocketView> GetSocketViews(this SlotView slotView)
        => (new[] { slotView?.LeftView, slotView?.RightView }).WhereNotNull();

    public static IEnumerable<SocketView> GetSocketViews(this NodeView nodeView)
        => nodeView.SlotViews.SelectMany(GetSocketViews);

    // In order to compute connection views, I need the socket locations, which means I need the slot locations (SlotViews)

    public static IReadOnlyList<ConnectionView> ToViews(this IReadOnlyList<Connection> connections, IReadOnlyList<NodeView> nodeViews)
    {
        var socketViews = nodeViews.SelectMany(GetSocketViews).ToDictionary(x => x.Id, x => x);

        var r = new List<ConnectionView>();
        foreach (var conn in connections)
        {
            socketViews.TryGetValue(conn.Source, out var sourceSocketView);
            socketViews.TryGetValue(conn.Destination, out var destinationSocketView);
            if (sourceSocketView != null && destinationSocketView != null)
                r.Add(conn.ToView(sourceSocketView.Point, destinationSocketView.Point));
            else
                throw new Exception("Could not find one of the socket views");
        }

        return r;
    }

    public static IReadOnlyList<SlotView> ToViews(this IReadOnlyList<Slot> slots, Point point, IReadOnlyList<SlotView>? views)
    {
        var slotLookup = slots.ToDictionary(n => n.Id, n => n);
        var viewLookup = views?.ToDictionary(v => v.Id, v => v) ?? new Dictionary<Guid, SlotView>();

        var result = new List<SlotView>();
        var rect = new Rect(point.X, point.Y + Dimensions.NodeHeaderHeight, Dimensions.NodeWidth, Dimensions.NodeSlotHeight);
        foreach (var slot in slots)
        {
            // TODO: what does oldView do here? This is not clear. 
            viewLookup.TryGetValue(slot.Id, out var oldView);
            var newView = slot.ToView(rect);
            rect.Offset(0, Dimensions.NodeSlotHeight);
            result.Add(newView);
        }

        return result;
    }

    public static GraphView ToView(this Graph self, GraphView? prev)
    {
        var nodeViews = self.Nodes.ToViews(prev?.NodeViews);
        var connectionViews = self.Connections.ToViews(nodeViews);
        return new GraphView(self, nodeViews, connectionViews);
    }

    public static Rect NodeHeaderRect(this Rect rect)
        => new(rect.TopLeft, new Size(Dimensions.NodeWidth, Dimensions.NodeHeaderHeight));

    public static NodeView ToView(this Node self, Rect rect, IReadOnlyList<SlotView> slotViews)
        => new(self, rect, rect.NodeHeaderRect(), slotViews);

    public static Point LeftCenter(this Rect rect)
        => new(rect.Left, rect.Top + rect.Height / 2);

    public static Point RightCenter(this Rect rect)
        => new(rect.Right, rect.Top + rect.Height / 2);

    public static SocketView ToView(this Socket self, Point point)
        => new(self, point);

    public static SlotView ToView(this Slot self, Rect rect)
        => new(self, rect, self.Left?.ToView(rect.LeftCenter()), self.Right?.ToView(rect.RightCenter()));

    public static ConnectionView ToView(this Connection self, Point a, Point b)
        => new(self, new(a, b));
}
