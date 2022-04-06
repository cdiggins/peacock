using System;
using System.Windows;
using Peacock;

namespace Emu.Controls;

public record SocketStyle(ShapeStyle ShapeStyle, TextStyle TextStyle, Radius Radius);

public record SocketView(Socket Socket, SocketStyle Style) : View(Socket, Socket.Id);

public record SocketControl(Rect Rect, SocketView View, Func<IUpdates, IControl, IControl, IUpdates> Callback) 
    : Control<SocketView>(Rect, View, Callback)
{
    public override ICanvas Draw(ICanvas canvas) 
        => canvas.Draw(StyledShape());

    public StyledEllipse StyledShape() 
        => new(View.Style.ShapeStyle, Shape());

    public Ellipse Shape() 
        => new(Measurements.RelativeRect.Center(), View.Style.Radius);

    public Point Center()
        => Measurements.AbsoluteRect.Center();
}