using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Ned;

public record DragState(bool IsDragging = false, Point ControlStart = new(), Point MouseDragStart = new());

public record ConnectingState(bool IsConnecting = false, Point Start = new(), Point Current = new(), Guid SocketId = new());

public static class Behaviors
{
    public static Control<NodeView> MoveTo(this Control<NodeView> control, Point point)
        => control.State.Node.ToView(control.State, control.State.Rect.MoveTo(point)).ToControl();

    public static double Sqr(double x)
        => x * x;

    public static double DistanceSqr(Point a, Point b)
        => Sqr(a.X - b.X) + Sqr(a.Y - b.Y);

    public static double MinDistSqr()
        => Dimensions.SocketRadius.X * Dimensions.SocketRadius.Y;

    public static bool Connected(SocketView s, Point p)
        => DistanceSqr(s.Point, p) < MinDistSqr();

    public static ICanvas DrawNewConnection(ICanvas canvas, Point start, Point end)
        => canvas.Draw(new StyledLine(new PenStyle(new BrushStyle(Colors.Blue), 5), new(start, end)));

    public static Control<GraphView> AddConnectingBehavior(this Control<GraphView> graph)
    {
        var behavior = new Behavior<ConnectingState>(new(),  
            (control, canvas, state) =>            
                 state.IsConnecting
                    ? DrawNewConnection(canvas, state.Start, state.Current)
                    : canvas
            ,
            (control, input, state) =>
            {
                if (!state.IsConnecting && input is MouseDownEvent mde)
                {
                    var mousePoint = mde.MouseStatus.Location;
                    var sockets = graph.State.GetSocketViews();
                    var socket = sockets.FirstOrDefault(s => Connected(s, mousePoint));
                    if (socket != null)
                    {
                        return (control, state with { IsConnecting = true, SocketId = socket.Id, Start = socket.Point, Current = mousePoint });
                    }
                }
                else if (state.IsConnecting && input is MouseMoveEvent mme)
                {
                    return (control, state with { Current = mme.MouseStatus.Location });
                }    
                
                return (control, state);
            });
        return (Control<GraphView>)graph.AddBehavior(behavior);
    }

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
