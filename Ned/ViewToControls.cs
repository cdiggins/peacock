using System;
using System.Collections.Generic;
using System.Linq;

namespace Ned;

public static class ViewToControls
{
    // TODO: the child generating function needs to support updating the state. 

    public static Control<T> ToControl<T>(
        this T state,
        Func<ICanvas, T, ICanvas>? drawFunc = null,
        Func<T, IEnumerable<IControl>>? childrenFunc = null
        )
        where T : class, IView
        => new(state, 1, drawFunc, childrenFunc);

    public static Control<GraphView> ToControl(this GraphView self)
        => self.ToControl(ViewDrawing.DrawGraph, (view) => view.NodeViews.Select(ToControl).Cast<IControl>().Concat(view.ConnectionViews.Select(ToControl)))
        .AddConnectingBehavior();

    public static Control<NodeView> ToControl(this NodeView self)
        => self.ToControl(ViewDrawing.DrawNode, (view) => view.SlotViews.Select(ToControl).Cast<IControl>().Append(view.HeaderView.ToControl()))
        .AddDraggingBehavior();
       
    public static Control<HeaderView> ToControl(this HeaderView self)
        => self.ToControl(ViewDrawing.DrawHeader, (view) => ToControls(view.LeftView, view.RightView));

    public static IEnumerable<IControl> ToControls(SocketView? a, SocketView? b)
        => (new[] { a?.ToControl(), b?.ToControl() }).WhereNotNull();

    public static Control<SlotView> ToControl(this SlotView self)
        => self.ToControl(ViewDrawing.DrawSlot, (view) => ToControls(view.LeftView, view.RightView));

    public static Control<SocketView> ToControl(this SocketView self)
        => self.ToControl(ViewDrawing.DrawSocket);

    public static Control<ConnectionView> ToControl(this ConnectionView self) 
        => new(self, -1, ViewDrawing.DrawConnection);

    public static IControl ToControl(this GraphView self, IControl? prev)
    {
        var typedPrev = prev as Control<GraphView>;
        if (typedPrev?.View.Id == self.Id)
        {
            return typedPrev.UpdateView(self);
        }
        return self.ToControl(ViewDrawing.DrawGraph, 
            (view) => view.NodeViews.Select(ToControl).Cast<IControl>().Concat(view.ConnectionViews.Select(ToControl)))
        .AddConnectingBehavior();
    }
}

