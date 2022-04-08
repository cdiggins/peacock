using System.Windows;
using System.Windows.Input;
using Emu.Controls;
using Peacock;

namespace Emu.Behaviors;

public enum Corner
{ 
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
}

public record ResizingState(bool IsResizing, Rect ControlRect, Point MouseDragStart, Corner Corner)
{
    public ResizingState() : this(false, new(), new(), Corner.BottomRight) { }
}

public record ResizingBehavior(object? ControlId)
    : Behavior<ResizingState>(ControlId)
{
    public IUpdates CancelResize(IUpdates updates)
        => UpdateState(updates, x => x with { IsResizing = false });

    public IUpdates StartResize(IUpdates updates, Rect controlRect, Point dragStart, Corner corner)
        => UpdateState(updates, x => x with { IsResizing = true, ControlRect = controlRect, MouseDragStart = dragStart, Corner = corner });

    public static Rect AddOffsetToCorner(Rect rect, Point offset, Corner corner)
        =>
        corner switch
        {
            Corner.TopLeft => rect.Ad
        }


    public override IUpdates Process(IControl control, InputEvent input, IUpdates updates)
    {
        if (control is not NodeControl nodeControl)
            return base.Process(control, input, updates);

        if (State.IsResizing)
        {
            switch (input)
            {
                case MouseUpEvent:
                    return CancelResize(updates);

                case MouseMoveEvent mme:
                    {
                        if (!input.MouseStatus.LButtonDown)
                            return CancelResize(updates);

                        var offset = mme.MouseStatus.Location.Subtract(State.MouseDragStart);
                        var newRect = State.ControlStart.Add(offset);

                        return updates.UpdateModel(nodeControl.View.Node,
                            model => model with { Rect = model.Rect.MoveTo(newLocation) });
                    }
            }
        }
        else
        {
            if (input is MouseDownEvent)
            {
                var location = input.MouseStatus.Location;

                if (nodeControl.Absolute.Contains(location))
                {
                    var socket = nodeControl.HitSocket(location);
                    if (socket == null)
                        return StartResize(updates, nodeControl.View.Node.Rect.TopLeft, input.MouseStatus.Location);
                }
            }
        }

        return updates;
    }
}
