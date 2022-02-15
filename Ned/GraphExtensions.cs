using System;
using System.Collections.Generic;
using System.Linq;

namespace Ned
{
    public static class GraphExtensions
    {
        public static IEnumerable<Socket> GetSockets(this Graph g)
            => g.Nodes.SelectMany(GetSockets);

        public static IEnumerable<Socket> GetSockets(this Node n)
            => n.Slots.SelectMany(GetSockets);

        public static IEnumerable<Socket> GetSockets(this Slot s)
            => (new[] { s.Left, s.Right }).WhereNotNull();

        public static IReadOnlyDictionary<Guid, T> ToGuidLookup<T>(this IEnumerable<T> xs) where T : Element
            => xs.ToDictionary(x => x.Id, x => x);

        public static IReadOnlyDictionary<Guid, Socket> GetSocketLookup(this Graph g)
            => g.GetSockets().ToGuidLookup();

        public static IEnumerable<SocketView> GetSocketViews(this GraphView g)
            => g.NodeViews.SelectMany(GetSocketViews);

        public static IEnumerable<SocketView> GetSocketViews(this NodeView n)
            => n.SlotViews.SelectMany(GetSocketViews);

        public static IEnumerable<SocketView> GetSocketViews(this SlotView s)
            => (new[] { s.LeftView, s.RightView }).WhereNotNull();
    }
}
