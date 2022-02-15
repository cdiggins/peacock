using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Ned;

public static class ViewDrawing
{
    public static Shapes Shapes => new(new());

    public static ICanvas Draw(this ICanvas canvas, GraphView view)
        => canvas;

    public static ICanvas Draw(this ICanvas canvas, NodeView view)
        => canvas
            .Draw(Shapes.NodeShadow(view))
            .Draw(Shapes.StyledShape(view));
    
    public static ICanvas Draw(this ICanvas canvas, HeaderView view)
        => canvas.Draw(Shapes.StyledShape(view)).Draw(Shapes.StyledText(view));

    public static ICanvas Draw(this ICanvas canvas, SlotView view)
        => canvas.Draw(Shapes.StyledShape(view)).Draw(Shapes.StyledText(view));

    public static ICanvas Draw(this ICanvas canvas, SocketView view)
        => canvas.Draw(Shapes.StyledShape(view));

    public static ICanvas Draw(this ICanvas canvas, ConnectionView view)
        => canvas.Draw(Shapes.StyledLine(view));
}

