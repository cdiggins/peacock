using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
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

public record Node(Guid Id, Rect Rect, string Label, NodeKind Kind, Slot Header, IReadOnlyList<Slot> Slots) : Model(Id)
{
}

public record Slot(Guid Id, Rect Rect, string Label, string Type, bool IsHeader, Socket? Left, Socket? Right) : Model(Id)
{
    public Rect GetAbsoluteRect(Node Node)
        => Rect.MoveBy(Node.Rect.TopLeft);
}

public record Socket(Guid Id, Rect Rect, string Type, bool LeftOrRight) : Model(Id)
{
    public Rect GetAbsoluteRect(Node node)
        => Rect.MoveBy(node.Rect.TopLeft);
}

public record Connection(Guid Id, Line Line, Guid SourceId, Guid DestinationId) : Model(Id);

public record Graph(Guid Id, IReadOnlyList<Node> Nodes, IReadOnlyList<Connection> Connections) : Model(Id);

/// <summary>
/// This class contains functions for applying proposed changes from an IUpdates class to the model.
/// </summary>
public static class ModelUpdateExtensions
{
    public static Graph UpdateModel(this IUpdates updates, Graph graph) => 
        updates.ApplyToModel(graph with
        {
            Nodes = graph.Nodes.Select(x => UpdateModel(updates, x)).ToList(),
            Connections = graph.Connections.Select(x => UpdateModel(updates, x)).ToList()
        });

    public static Connection UpdateModel(this IUpdates updates, Connection conn) => 
        updates.ApplyToModel(conn);

    public static Socket? UpdateModel(this IUpdates updates, Socket? sock)
        => sock == null ? null : updates.ApplyToModel(sock);

    public static Slot UpdateModel(this IUpdates updates, Slot slot)
        => updates.ApplyToModel(slot with
        {
            Left = UpdateModel(updates, slot.Left),
            Right = UpdateModel(updates, slot.Right)
        });

    public static Node UpdateModel(this IUpdates updates, Node node)
        => updates.ApplyToModel(node with
        {
            Slots = node.Slots.Select(slot => UpdateModel(updates, slot)).ToList(),
            Header = UpdateModel(updates, node.Header),
        });
}
