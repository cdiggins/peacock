using System.Collections.Generic;
using System.Linq;

namespace Ned;

public static class ViewToControls
{ 
    public static Control<GraphView> ToControl(this GraphView view)
        => new(view, (canvas, view) => canvas.Draw(view), view.NodeViews.Select(ToControl).Cast<IControl>().Concat(view.ConnectionViews.Select(ToControl)));

    public static Control<NodeView> ToControl(this NodeView view)
        => new Control<NodeView>(view, (canvas, view) => canvas.Draw(view), view.SlotViews.Select(ToControl).Cast<IControl>().Append(view.HeaderView.ToControl()))
        .AddDraggingBehavior();

    public static Control<HeaderView> ToControl(this HeaderView view)
        => new(view, (canvas, view) => canvas.Draw(view), ToControls(view.LeftView, view.RightView));

    public static IEnumerable<IControl> ToControls(SocketView? a, SocketView? b)
        => (new[] { a?.ToControl(), b?.ToControl() }).WhereNotNull();

    public static Control<SlotView> ToControl(this SlotView view)
        => new(view, (canvas, view) => canvas.Draw(view), ToControls(view.LeftView, view.RightView));

    public static Control<SocketView> ToControl(this SocketView view) 
        => new(view, (canvas, view) => canvas.Draw(view));

    public static Control<ConnectionView> ToControl(this ConnectionView view) 
        => new(view, (canvas, view) => canvas.Draw(view));
}

