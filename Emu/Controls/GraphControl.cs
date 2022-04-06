using System;
using System.Collections.Generic;
using System.Windows;
using Emu.Behaviors;
using Peacock;

namespace Emu.Controls;

public record GraphStyle(ShapeStyle ShapeStyle, TextStyle TextStyle);

public record GraphView(Graph Graph, GraphStyle Style) : View(Graph, Graph.Id);

public record GraphControl(GraphView View,
    IReadOnlyList<NodeControl> Nodes,
    IReadOnlyList<ConnectionControl> Connections,
    Func<IUpdates, IControl, IControl, IUpdates> Callback)
    : Control<GraphView>(Measures.Default, View, ToChildren(Nodes, Connections), Callback)
{
    public override IEnumerable<IBehavior> GetDefaultBehaviors()
        => new[] { new ConnectingBehavior(this) };
}