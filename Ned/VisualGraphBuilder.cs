using Peacock;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            => (new[] { s.Left, s.Right }).Where(s => s != null);

        public static IReadOnlyDictionary<Guid, T> ToGuidLookup<T>(this IEnumerable<T> xs) where T : Element
            => xs.ToDictionary(x => x.Id, x => x);

        public static IReadOnlyDictionary<Guid, Socket> GetSocketLookup(this Graph g)
            => g.GetSockets().ToGuidLookup();
    }
}
