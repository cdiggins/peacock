using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Emu.Behaviors;
using Peacock;

namespace Emu.Controls;

public record NodeStyle(ShapeStyle ShapeStyle, TextStyle TextStyle, Radius Radius, Color ShadowColor);

public record NodeView(Node Node, NodeStyle Style) : View(Node, Node.Id);

public record NodeControl(Measures Measures, NodeView View, SlotControl Header, IReadOnlyList<SlotControl> Slots, Func<IUpdates, IControl, IControl, IUpdates> Callback) 
    : Control<NodeView>(Measures, View, Slots.Prepend(Header).ToList(), Callback)
{
    public StyledRect StyledShape() 
        => new(View.Style.ShapeStyle, Shape());

    public RoundedRect Shape() 
        => new(Client, View.Style.Radius);

    public override ICanvas Draw(ICanvas canvas)
        //=> canvas.Draw(StyledShape());
        => canvas;

    public override IEnumerable<IBehavior> GetDefaultBehaviors()
        => new IBehavior[] { new DraggingBehavior(this), new ResizingBehavior(this), new ShadowBehavior(this) };
}