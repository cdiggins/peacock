using System.Linq;

namespace Ned;

public static class ViewToControls
{ 
    public static Control<GraphView> ToControl(this GraphView view)
        => new(view, (canvas, view) => canvas.Draw(view), view.NodeViews.Select(ToControl).Cast<IControl>().Concat(view.ConnectionViews.Select(ToControl)));

    public static Control<NodeView> ToControl(this NodeView view)
        => new Control<NodeView>(view, (canvas, view) => canvas.Draw(view), view.SlotViews.Select(ToControl)).AddDraggingBehavior();

    public static Control<SlotView> ToControl(this SlotView view)
        => new(view, (canvas, view) => canvas.Draw(view), (new[] { view.LeftView?.ToControl(), view.RightView?.ToControl() }).WhereNotNull());

    public static Control<SocketView> ToControl(this SocketView view) 
        => new(view, (canvas, view) => canvas.Draw(view));

    public static Control<ConnectionView> ToControl(this ConnectionView view) 
        => new(view, (canvas, view) => canvas.Draw(view));
}

