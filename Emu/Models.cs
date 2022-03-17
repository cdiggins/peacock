using Peacock;

namespace Emu;

public enum NodeKind
{
    PropertySet,
    OperatorSet,
    Input,
    Output,
}

public record LabeledObject(string Label) : Object;
public record Node(string Label, NodeKind Kind, Ref<Slot> Header, RefList<Slot> Slots) : LabeledObject(Label);
public record Slot(string Name, string Type, bool IsHeader, Ref<Socket>? Left, Ref<Socket>? Right): LabeledObject(Name);
public record Socket(string Type, bool LeftOrRight) : Object;
public record Connection(Ref<Socket> Source, Ref<Socket> Destination) : Object;
public record Graph(RefList<Node> Nodes, RefList<Connection> Connections)  : Object;