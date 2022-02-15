using System;
using System.Collections.Generic;
using System.Windows;

namespace Ned
{
    // A view is the strict set of data required to provide an interactive visual representation. 
    // The details of how to style the representation (fonts,colors,brush-strokes,etc.) are 
    // separated by another functions that maps these things to controls. 

    public record ElementView(
        Element? Element)
    {
        public Guid Id => Element?.Id ?? Guid.Empty;
    }

    public record ConnectionView(
        Connection? Connection,
        Line Line)
        : ElementView(Connection);

    public record HeaderView(
        Header Header,
        Rect Rect,
        SocketView? LeftView,
        SocketView? RightView
        ) : ElementView(Header);

    public record NodeView(Node Node) : ElementView(Node)
    {
        public Rect Rect { get; init; } = new();
        public HeaderView HeaderView { get; init; }
        public IReadOnlyList<SlotView> SlotViews { get; init; } = Array.Empty<SlotView>();
    }

    public record SlotView(
        Slot Slot, 
        Rect Rect, 
        SocketView? LeftView, 
        SocketView? RightView) 
        : ElementView(Slot);

    public record SocketView(
        Socket Socket, 
        Point Point) 
        : ElementView(Socket);

    public record GraphView(
        Graph Graph, 
        IReadOnlyList<NodeView> NodeViews,
        IReadOnlyList<ConnectionView> ConnectionViews) 
        : ElementView(Graph);
}