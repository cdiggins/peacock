using System;
using System.Collections.Generic;

namespace Ned;

public record Element
{
    public Guid Id { get; init; } = Guid.NewGuid();
}

public record LabeledElement(string Label) : Element;

public record Graph(IReadOnlyList<Node> Nodes, IReadOnlyList<Connection> Connections) : Element;

public record Node(string Label, NodeKind Kind, Header Header, IReadOnlyList<Slot> Slots) : LabeledElement(Label);

public record Slot(string Name, string Type, Socket? Left, Socket? Right) : LabeledElement(Name);

public enum NodeKind
{
    PropertySet,
    OperatorSet,
    Input,
    Output,
}

public record Header(string Label, Socket? Left, Socket ? Right) : Slot(Label, Label, Left, Right);

public record Socket(string Type, bool LeftOrRight) : Element;

public record Connection(Guid Source, Guid Destination) : Element;