using System;
using System.Linq;
using System.Windows;

namespace Ned;

public record DragState(bool IsDragging = false, Point ControlStart = new(), Point MouseDragStart = new());

public static class Behaviors
{
    // TODO: I need a generic way to update the children based on the new view. 
    // I notice that ViewToControls does something very similar, so there is risk of screwing this up.
    // Maybe the control needs to have the constructor function embedded in it.
    // I think the control needs to know how to generate a control from a view ... so it can apply it recursively to its children.


    public static Control<NodeView> UpdateState(this Control<NodeView> node, NodeView view)
        => node with { State = view, Children = view.SlotViews.Select(ViewToControls.ToControl).ToList() };

    public static Control<NodeView> MoveTo(this Control<NodeView> control, Point point)
        => control.UpdateState(control.State.SetLayout(control.State.Rect.MoveTo(point)));

    public static Control<NodeView> AddDraggingBehavior(this Control<NodeView> control)
    {
        var behavior = new Behavior<DragState>(new(), null, (control, input, state) =>
        {
            var nodeViewControl = (Control<NodeView>)control;
            if (state.IsDragging)
            {
                if (input is MouseUpEvent mue)
                {
                    state = state with { IsDragging = false };
                }
                else if (input is MouseMoveEvent mme)
                {
                    var offset = mme.MouseStatus.Location.Subtract(state.MouseDragStart);
                    var newLocation = state.ControlStart.Add(offset);
                    nodeViewControl = nodeViewControl.MoveTo(newLocation);
                }
            }
            else
            {
                if (input is MouseDownEvent md && nodeViewControl.State.Rect.Contains(input.MouseStatus.Location))
                {
                    state = state with { 
                        IsDragging = true, 
                        ControlStart = nodeViewControl.State.Rect.Location, 
                        MouseDragStart = input.MouseStatus.Location 
                    };
                }
            }

            return (nodeViewControl, state);
        });

        return (Control<NodeView>)control.AddBehavior(behavior);
    }
}
