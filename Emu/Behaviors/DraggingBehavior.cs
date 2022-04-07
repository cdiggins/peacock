using System.Windows;
using Emu.Controls;
using Peacock;

namespace Emu.Behaviors;

public record DragState(bool IsDragging, Point ControlStart, Point MouseDragStart)
{
    public DragState() : this(false, new(), new()) { }
}

public record DraggingBehavior(NodeControl NodeControl) : Behavior<DragState>(NodeControl)
{
    public override IUpdates Process(InputEvent input, IUpdates updates)
    {
        if (State.IsDragging)
        {
            switch (input)
            {
                case MouseUpEvent:
                    return UpdateState(updates, x => x with { IsDragging = false });

                case MouseMoveEvent mme:
                {
                    var offset = mme.MouseStatus.Location.Subtract(State.MouseDragStart);
                    var newLocation = State.ControlStart.Add(offset);

                    return updates.UpdateModel(NodeControl.View.Node,
                        model => model with { Rect = model.Rect.MoveTo(newLocation) });
                }
            }
        }
        else
        {
            if (input is MouseDownEvent)
            {
                var location = input.MouseStatus.Location;

                // TODO: will need to check that we aren't hitting a socket.

                if (NodeControl.Absolute.Contains(location))
                {
                    return UpdateState(updates, x => x with
                    {
                        IsDragging = true,
                        ControlStart = NodeControl.View.Node.Rect.TopLeft,
                        MouseDragStart = input.MouseStatus.Location
                    });
                }
            }
        }

        return updates;
    }
}