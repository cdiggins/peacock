using System;
using System.Collections.Generic;
using System.Linq;
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

public record Model(Guid Id) : IModel;

public record Node(Guid Id, Rect Rect, string Label, NodeKind Kind, Slot Header, IReadOnlyList<Slot> Slots) : Model(Id);

public record Slot(Guid Id, Rect Rect, string Label, string Type, bool IsHeader, Socket? Left, Socket? Right) : Model(Id);

public record Socket(Guid Id, Rect Rect, string Type, bool LeftOrRight) : Model(Id);

public record Connection(Guid Id, Line Line, Guid SourceId, Guid DestinationId) : Model(Id);

public record Graph(Guid Id, IReadOnlyList<Node> Nodes, IReadOnlyList<Connection> Connections) : Model(Id);

/// <summary>
/// This class contains functions for applying proposed changes from an IUpdates class to the model.
/// </summary>
public static class ModelUpdateExtensions
{
    public static Graph Apply(this IUpdates updates, Graph graph) => 
        updates.ApplyToModel(graph with
        {
            Nodes = graph.Nodes.Select(updates.Apply).ToList(),
            Connections = graph.Connections.Select(updates.Apply).ToList()
        });

    public static Connection Apply(this IUpdates updates, Connection conn) => 
        updates.ApplyToModel(conn);

    public static Socket? Apply(this IUpdates updates, Socket? sock)
        => sock == null ? null : updates.ApplyToModel(sock);

    public static Slot Apply(this IUpdates updates, Slot slot)
        => updates.ApplyToModel(slot with
        {
            Left = Apply(updates, slot.Left),
            Right = Apply(updates, slot.Right)
        });

    public static Node Apply(this IUpdates updates, Node node)
        => updates.ApplyToModel(node with
        {
            Slots = node.Slots.Select(updates.Apply).ToList(),
            Header = Apply(updates, node.Header),
        });
}
