using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ned
{
    public record Element
    {
        public Guid Id { get; init; } = Guid.NewGuid();
    }

    public record LabeledElement : Element
    {
        public string Label { get; init; } = "";
    }

    public record Graph : LabeledElement
    {
        public IReadOnlyList<Node> Nodes { get; init; } = Array.Empty<Node>();
        public IReadOnlyList<Connection> Connections { get; init; } = Array.Empty<Connection>();
    }

    public record Node : LabeledElement
    { 
        public IReadOnlyList<Slot> Slots { get; init; } = Array.Empty<Slot>();
    }

    public record Slot : LabeledElement
    {
        public Socket? Left { get; init; }
        public Socket? Right { get; init; }       
    }

    public record Socket : LabeledElement
    {
        public bool LeftOrRight { get; init;}
    }

    public record Connection : LabeledElement
    {
        public Guid Source { get; init; } = Guid.Empty;
        public Guid Destination { get; init; } = Guid.Empty;
    }    
}
