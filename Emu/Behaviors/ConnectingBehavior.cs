using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Emu.Controls;
using Peacock;

namespace Emu.Behaviors;

public record ConnectingState(bool IsDragging, SocketControl? Source, Point Current, bool StartingFromSource)
{
    public ConnectingState()
        : this(false, null, new(), true)
    { }

    public Point SourcePoint => Source?.AbsoluteCenter() ?? new();
    public Point StartPoint => StartingFromSource ? SourcePoint : Current;
    public Point EndPoint => StartingFromSource ? Current : SourcePoint;
}

public record ConnectingBehavior(object? ControlId) : Behavior<ConnectingState>(ControlId)
{
    public override IUpdates Process(IControl control, InputEvent input, IUpdates updates)
    {
        var graphControl = control as GraphControl;
        if (graphControl == null)
            return base.Process(control, input, updates);

        if (!State.IsDragging && input is MouseDownEvent mde)
        {
            var mousePoint = mde.MouseStatus.Location;
            var socket = graphControl.HitSocket(mousePoint);
            if (socket != null)
            {
                return UpdateState(updates, x => x with { 
                    IsDragging = true, 
                    Source = socket, 
                    Current = mousePoint, 
                    StartingFromSource = !socket.View.Socket.LeftOrRight
                });
            }
        }
        else if (State.IsDragging)
        {
            if (input is MouseMoveEvent mme)
            {
                if (!input.MouseStatus.LButtonDown)
                    return UpdateState(updates, x => x with { IsDragging = false });

                return UpdateState(updates, x => x with {
                    Current = mme.MouseStatus.Location
                });
            }
            if (input is MouseUpEvent mue)
            {
                var endSocket = graphControl.ConnectableSocket(State);
                if (endSocket != null && State.Source != null)
                {
                    var sourceId = State.Source.View.Socket.Id;
                    var destId = endSocket.View.Socket.Id;
                    if (State.Source.View.Socket.LeftOrRight)
                    {
                        (sourceId, destId) = (destId, sourceId);
                    }

                    updates = updates.UpdateControl(graphControl, 
                        localControl => ((GraphControl)localControl).AddConnection(sourceId, destId));
                }

                return UpdateState(updates, x => x with { IsDragging = false });
            }
        }
        return base.Process(control, input, updates);
    }

    public override ICanvas PostDraw(ICanvas canvas, IControl control)
        => State.IsDragging
            ? canvas.Draw(new(Colors.Transparent), new(Colors.Blue, 4),
                ConnectionControl.ConnectorGeometry(State.SourcePoint, State.EndPoint))
            : base.PostDraw(canvas, control);
}

public static class ConnectingBehaviorExtensions
{
    public static double Sqr(double x)
        => x * x;

    public static double DistanceSqr(Point a, Point b)
        => Sqr(a.X - b.X) + Sqr(a.Y - b.Y);

    public static bool CloseEnough(this SocketControl s, Point p)
        => DistanceSqr(s.AbsoluteCenter(), p) < 5;

    public static bool CanConnect(this SocketControl socket, ConnectingState state)
        => state.Source != null && CloseEnough(socket, state.Current) && Semantics.CanConnect(state.Source.View.Socket, socket.View.Socket);

    public static IEnumerable<SocketControl> GetSockets(this IControl control)
        => control.Descendants().OfType<SocketControl>();

    public static SocketControl? GetSocket(this IControl control, Func<SocketControl, bool> predicate)
        => control.GetSockets().FirstOrDefault(predicate);

    public static SocketControl? ConnectableSocket(this IControl control, ConnectingState state)
        => control.GetSocket(s => s.CanConnect(state));

    public static SocketControl? HitSocket(this IControl control, Point p)
        => control.GetSocket(s => s.CloseEnough(p));

    public static GraphControl AddConnection(this GraphControl control, Guid a, Guid b)
        => throw new NotImplementedException();

}