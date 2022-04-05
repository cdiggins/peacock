using System;
using System.Windows;
using Peacock;

namespace Emu;

public record DragState(bool IsDragging = false, Point ControlStart = new(), Point MouseDragStart = new());

public record ConnectingState(bool IsDragging = false, SocketView? Source = null, Point Current = new(), bool StartingFromSource = true)
{
    public Point SourcePoint => Source?.Socket.Rect.Center() ?? new();
    public Point StartPoint => StartingFromSource ? SourcePoint : Current;
    public Point EndPoint => StartingFromSource ? Current : SourcePoint;
}

public record DraggingBehavior(NodeControl NodeControl) : Behavior(NodeControl)
{
    public DragState State { get; init; } = new();

    public override IUpdates Process(InputEvent input, IUpdates updates)
    {
        if (State.IsDragging)
        {
            switch (input)
            {
                case MouseUpEvent:
                    return updates.UpdateBehavior(this, 
                        x => x with { State = State with { IsDragging = false } });

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

                if (NodeControl.View.Node.Rect.Contains(location))
                {
                    return updates.UpdateBehavior(this,
                        x => x with
                        {
                            State = State with
                            {
                                IsDragging = true,
                                ControlStart = NodeControl.View.Node.Rect.TopLeft,
                                MouseDragStart = input.MouseStatus.Location
                            }
                        });
                }
            }
        }

        return updates;
    }
}

public static class Behaviors
{
    public static IUpdates UpdateModel<TModel>(this IUpdates updates, TModel model, Func<TModel, TModel> func)
        where TModel : IModel
        => updates.UpdateModel(model, m => func((TModel)m));

    public static IUpdates UpdateBehavior<TBehavior>(this IUpdates updates, TBehavior behavior, Func<TBehavior, TBehavior> func)
        where TBehavior : IBehavior
        => updates.UpdateBehavior(behavior, b => func((TBehavior)b));

    public static double Sqr(double x)
        => x * x;

    public static double DistanceSqr(Point a, Point b)
        => Sqr(a.X - b.X) + Sqr(a.Y - b.Y);

    public static bool CloseEnough(SocketView s, Point p)
        => DistanceSqr(s.Socket.Rect.Center(), p) < 5;

    public static bool CanConnect(SocketView view, ConnectingState state)
        => state.Source != null && CloseEnough(view, state.Current) && Semantics.CanConnect(state.Source.Socket, view.Socket);

    /*
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
    */
}