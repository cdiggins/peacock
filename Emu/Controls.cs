using Peacock;

namespace Emu;

public record GraphControl(GraphView View) 
    : BaseControl<GraphView>(View);

public record SocketControl(SocketView View) 
    : BaseControl<SocketView>(View)
{
    public override ICanvas Draw(ICanvas canvas) 
        => canvas.DrawSocket(View);
}

public record SlotControl(SlotView View) 
    : BaseControl<SlotView>(View)
{
    public override ICanvas Draw(ICanvas canvas) 
        => canvas.DrawSlot(View);
}

public record NodeControl(NodeView View) 
    : BaseControl<NodeView>(View)
{
    public override ICanvas Draw(ICanvas canvas) 
        => canvas.DrawNode(View);
}

public record ConnectionControl(ConnectionView View)
    : BaseControl<ConnectionView>(View)
{
    public override ICanvas Draw(ICanvas canvas) 
        => canvas.DrawConnection(View);
}

/*
public record DragState(bool IsDragging = false, Point ControlStart = new(), Point MouseDragStart = new());

public record ConnectingState(bool IsDragging = false, SocketView? Source = null, Point Current = new(), bool StartingFromSource = true)
{
    public Point SourcePoint => Source?.Point ?? new();
    public Point StartPoint => StartingFromSource ? SourcePoint : Current;
    public Point EndPoint => StartingFromSource ? Current : SourcePoint;
}

public static class Behaviors
{
    public static double Sqr(double x)
        => x * x;

    public static double DistanceSqr(Point a, Point b)
        => Sqr(a.X - b.X) + Sqr(a.Y - b.Y);

    public static double MinDistSqr()
        => (Dimensions.SocketRadius.X) * (Dimensions.SocketRadius.Y + 1);

    public static bool CloseEnough(SocketView s, Point p)
        => DistanceSqr(s.Point, p) < MinDistSqr();

    public static bool CanConnect(SocketView view, ConnectingState state)
        => state.Source != null && CloseEnough(view, state.Current) && Semantics.CanConnect(state.Source.Socket, view.Socket);

    public static SocketView? HitSocket(GraphView graphView, ConnectingState state)
        => graphView.GetSocketViews().FirstOrDefault(socket => CanConnect(socket, state));

    public static Control<GraphView> AddConnectingBehavior(this Control<GraphView> control)
    {
        var behavior = new Behavior<ConnectingState>(
            1,
            new(),  
            (control, canvas, state) =>            
                 state.IsDragging && state.Source != null
                    ? ViewDrawing.DrawConnection(canvas, state.StartPoint, state.EndPoint, Colors.BlueViolet)
                    : canvas
            ,
            (control, updates, input, state) =>
            {
                var graphControl = (Control<GraphView>)control;
                if (!state.IsDragging && input is MouseDownEvent mde)
                {
                    var mousePoint = mde.MouseStatus.Location;
                    var sockets = graphControl.View.GetSocketViews();
                    var socket = sockets.FirstOrDefault(s => CloseEnough(s, mousePoint));
                    if (socket != null)
                    {
                        return (updates, state with { IsDragging = true, Source = socket, Current = mousePoint, StartingFromSource = !socket.Socket.LeftOrRight });
                    }
                }
                else if (state.IsDragging)
                {
                    if (input is MouseMoveEvent mme)
                    {
                        // TODO: highlight any hovered slot / connector 
                        return (updates, state with { Current = mme.MouseStatus.Location });
                    }
                    else if (input is MouseUpEvent mue)
                    {
                        var endSocket = HitSocket(graphControl.View, state);
                        if (endSocket != null && state.Source != null)
                        {
                            var sourceId = state.Source.Id;
                            var destId = endSocket.Id;
                            if (state.Source.Socket.LeftOrRight)
                            {
                                (sourceId, destId) = (destId, sourceId);
                            }

                            updates = updates.AddUpdate(
                                (Control<GraphView>)control,
                                view => view.AddConnection(new(sourceId, destId)));
                        }
                        return (updates, state with { IsDragging = false });
                    }
                }
                
                return (updates, state);
            });
        return control.AddBehavior(behavior);
    }

    public static Control<NodeView> AddDraggingBehavior(this Control<NodeView> control)
    {
        var behavior = new Behavior<DragState>(0, new(), null, (control, updates, input, state) =>
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

                    updates.AddUpdate((Control<NodeView>)control,
                        view => view.WithRect(view.Rect.MoveTo(newLocation)));
                }
            }
            else
            {
                if (input is MouseDownEvent md)
                {
                    var location = input.MouseStatus.Location;

                    if (nodeViewControl.View.Rect.Contains(location)
                       && !nodeViewControl.View.GetSocketViews().Any(s => CloseEnough(s, location)))
                    {
                        state = state with
                        {
                            IsDragging = true,
                            ControlStart = nodeViewControl.View.Rect.Location,
                            MouseDragStart = input.MouseStatus.Location
                        };
                    }
                }
            }

            return (updates, state);
        });

        return control.AddBehavior(behavior);
    }
}
*/
