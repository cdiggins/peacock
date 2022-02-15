using System.Collections.Generic;
using System.Windows;

namespace Ned;

public static class ViewLayout
{
    public static SlotView SetLayout(this SlotView slot, Rect rect)
        => slot with
        {
            Rect = rect,
            LeftView = slot.LeftView.SetLayout(rect.LeftCenter()),
            RightView = slot.RightView.SetLayout(rect.RightCenter())
        };

    public static NodeView SetLayout(this NodeView nodeView, Rect rect)
    {
        var slotViews = new List<SlotView>();
        var localRect = new Rect(rect.X, rect.Y + Dimensions.NodeHeaderHeight, rect.Width, Dimensions.NodeSlotHeight);
        foreach (var slot in nodeView.SlotViews)
        {
            var newView = slot.SetLayout(localRect);
            localRect = localRect.MoveBy(new(0, Dimensions.NodeSlotHeight));
            slotViews.Add(newView);
        }
        return nodeView with { Rect = rect, SlotViews = slotViews };
    }

    public static SocketView? SetLayout(this SocketView? socket, Point point)
        => socket == null ? socket : socket with { Point = point };
}
