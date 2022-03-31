using System;
using System.Collections.Generic;
using System.Windows;
using Peacock;

namespace Emu;

public enum NodeKind
{
    PropertySet,
    OperatorSet,
    Input,
    Output,
}

[Mutable]
public class Guids
{
    public Guid New()
        => Guid.NewGuid();
}

public record Model(Guid Id) : IModel;

public record Node(Guid Id, Rect Rect, string Label, NodeKind Kind, Slot Header, IReadOnlyList<Slot> Slots) : Model(Id);

public record Slot(Guid Id, Rect Rect, string Label, string Type, bool IsHeader, Socket? Left, Socket? Right) : Model(Id);

public record Socket(Guid Id, Rect Rect, string Type, bool LeftOrRight) : Model(Id);

public record Connection(Guid Id, Line Line, Guid SourceId, Guid DestinationId) : Model(Id);

public record Graph(Guid Id, IReadOnlyList<Node> Nodes, IReadOnlyList<Connection> Connections) : Model(Id);
