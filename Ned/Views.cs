using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Ned;

// A view is the strict set of data required to provide an interactive visual representation. 
// The details of how to style the representation (fonts,colors,brush-strokes,etc.) are 
// separated by another functions that maps these things to controls. 

public interface IView
{
    Guid Id { get; }
    IView Apply(Func<IView, IView> func);
}

public abstract record ElementView(Element? Element) : IView
{
    public Guid Id => Element?.Id ?? Guid.Empty;
    public abstract IView Apply(Func<IView, IView> func);
}

public abstract record ElementRectView(Element? Element, Rect Rect) : ElementView(Element);

public record ConnectionView(Connection? Connection, Line Line) : ElementView(Connection)
{
    public override IView Apply(Func<IView, IView> func)
        => func(this);
}

public record SlotView : ElementRectView
{
    public SlotView(Slot slot, Rect rect)
        : this(slot, rect, slot.Left?.ToView(rect), slot.Right?.ToView(rect))
    { }    

    public SlotView(Slot slot, Rect rect, SocketView? leftView, SocketView? rightView)
        : base(slot, rect)
    {
        LeftView = leftView;
        RightView = rightView;
    }

    public Slot Slot => (Slot)Element;
    public SocketView? LeftView { get; }
    public SocketView? RightView { get; }

    public override IView Apply(Func<IView, IView> func)
        => func(new SlotView(Slot, Rect, LeftView?.ApplyTyped(func), RightView?.ApplyTyped(func)));

    public SlotView WithRect(Rect rect)
        => new(Slot, rect);
}

public record HeaderView : SlotView
{
    public HeaderView(Header header, Rect rect, NodeKind kind)
        : this(header, rect, kind, header.Left?.ToView(rect), header.Right?.ToView(rect))
    { }

    public HeaderView(Header header, Rect rect, NodeKind kind, SocketView? leftView, SocketView? rightView)
        : base(header, rect, leftView, rightView)
    => Kind = kind;

    public Header Header => (Header)Element;
    public NodeKind Kind { get; }

    public override IView Apply(Func<IView, IView> func)
        => func(new HeaderView(Header, Rect, Kind, LeftView?.ApplyTyped(func), RightView?.ApplyTyped(func)));

    public HeaderView WithRect(Rect rect)
        => new(Header, rect, Kind);
}

public record NodeView : ElementRectView
{
    public NodeView(Node node, Rect rect)
        :   base(node, rect)
    {
        HeaderView = new HeaderView(node.Header, rect.NodeHeaderRect(), node.Kind);
        SlotViews = node.Slots.Select((slot, i) => slot.ToView(rect.NodeSlotRect(i))).ToList();
    }

    public NodeView(Node node, Rect rect, HeaderView headerView, IEnumerable<SlotView> slotViews)
        : base(node, rect)
    {
        HeaderView = headerView;
        SlotViews = slotViews.ToList();
    }

    public NodeView WithRect(Rect rect)
    {
        return new(Node, rect,
            HeaderView.WithRect(rect.NodeHeaderRect()),
            SlotViews.Select((slot, i) => slot.WithRect(rect.NodeSlotRect(i))));
    }    

    public Node Node => (Node)Element;
    public HeaderView HeaderView { get; }
    public IReadOnlyList<SlotView> SlotViews { get; }

    public override IView Apply(Func<IView, IView> func)
        => func(new NodeView(Node, Rect, HeaderView.ApplyTyped(func), SlotViews.Apply(func).ToList()));
}

public record SocketView(Socket Socket, Point Point) : ElementView(Socket)
{
    public override IView Apply(Func<IView, IView> func)
        => func(this);

    public SocketView WithPoint(Point point)
        => new(Socket, point);
}

public record GraphView : ElementView
{
    public GraphView(Graph graph, IEnumerable<NodeView> nodeViews, IEnumerable<ConnectionView> connectionViews)
        : base(graph)
    {
        NodeViews = nodeViews.ToList();
        ConnectionViews = connectionViews.ToList();
    }

    public GraphView(Graph graph, GraphView? prev = null)
        : base(graph)
    {
        var viewLookup = prev?.NodeViews.ToDictionary(v => v.Id, v => v) ?? new Dictionary<Guid, NodeView>();

        var newNodeViews = new List<NodeView>();
        var offsetX = 50;
        var offsetY = 50;
        var rowHeight = 300;

        for (var i = 0; i < graph.Nodes.Count; ++i)
        {
            var node = graph.Nodes[i];
            viewLookup.TryGetValue(node.Id, out var oldView);

            var rect = oldView?.Rect ??
                new Rect(
                    new Point(offsetX + (i / 2) * (Dimensions.NodeWidth + Dimensions.NodeSpacing), offsetY + (i % 2) * rowHeight),
                    new Size(Dimensions.NodeWidth, Dimensions.NodeHeight(node.Slots.Count)));

            newNodeViews.Add(node.ToView(rect));
        }
        NodeViews = newNodeViews;
        ConnectionViews = ComputeConnections();
    }

    public IReadOnlyList<ConnectionView> ComputeConnections()
    {
        // Get the positions of the socketer
        var socketViews = NodeViews.SelectMany(n => n.GetSocketViews()).ToDictionary(x => x.Id, x => x);
        var newConnectionViews = new List<ConnectionView>();
        foreach (var conn in Graph.Connections)
        {
            socketViews.TryGetValue(conn.Source, out var sourceSocketView);
            socketViews.TryGetValue(conn.Destination, out var destinationSocketView);
            if (sourceSocketView != null && destinationSocketView != null)
                newConnectionViews.Add(conn.ToView(sourceSocketView.Point, destinationSocketView.Point));
            else
                throw new Exception("Could not find one of the socket views");
        }
        return newConnectionViews;
    }

    public GraphView RecomputeConnections()
        => new(Graph, NodeViews, ComputeConnections());

    public Graph Graph => (Graph)Element;
    public IReadOnlyList<NodeView> NodeViews { get; }
    public IReadOnlyList<ConnectionView> ConnectionViews { get; }

    public override IView Apply(Func<IView, IView> func)
    { 
        var r = (GraphView)func(new GraphView(Graph, NodeViews.Apply(func), ConnectionViews.Apply(func)));
        return r.RecomputeConnections();
    }
}

public static class ViewExtensions
{
    public static Rect NodeFirstSlotRect(this Rect nodeRect)
        => new(
            new Point(nodeRect.Left, nodeRect.Top + Dimensions.NodeHeaderHeight), 
            new Size(nodeRect.Width, Dimensions.NodeSlotHeight));

    public static Rect NodeHeaderRect(this Rect nodeRect)
        => new(nodeRect.TopLeft, new Size(Dimensions.NodeWidth, Dimensions.NodeHeaderHeight));

    public static Point LeftCenter(this Rect rect)
        => new(rect.Left, rect.Top + rect.Height / 2);

    public static Point RightCenter(this Rect rect)
        => new(rect.Right, rect.Top + rect.Height / 2);

    public static SocketView ToView(this Socket self, Rect rect)
        => new(self, self.LeftOrRight ? rect.LeftCenter() : rect.RightCenter());

    public static HeaderView ToView(this Header self, Rect rect, NodeKind kind)
        => new(self, rect, kind);

    public static SlotView ToView(this Slot self, Rect rect)
        => new(self, rect);

    public static NodeView ToView(this Node node, Rect rect)
        => new(node, rect);

    public static ConnectionView ToView(this Connection self, Point a, Point b)
        => new(self, new(a, b));

    public static T? ApplyTyped<T>(this T? self, Func<IView, IView> func)
        where T : IView
        => (T?)self?.Apply(func);

    public static GraphView AddConnection(this GraphView view, Connection conn)
        => new GraphView(view.Graph.AddConnection(conn), view);

    public static Graph AddConnection(this Graph graph, Connection conn)
        => graph with { Connections = graph.Connections.Add(conn) };

    public static IReadOnlyList<T> Apply<T>(this IEnumerable<T> self, Func<IView, IView> f)
        where T : class, IView
        => self.Select(f).Cast<T>().ToList();

    public static Rect NodeSlotRect(this Rect rect, int i)
        => new(rect.Left, rect.Top + Dimensions.NodeHeaderHeight + i * Dimensions.NodeSlotHeight, rect.Width, Dimensions.NodeSlotHeight);
}