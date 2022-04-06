﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Emu.Behaviors;
using Peacock;

namespace Emu.Controls;

public record NodeStyle(ShapeStyle ShapeStyle, TextStyle TextStyle, Radius Radius, Color ShadowColor);

public record NodeView(Node Node, NodeStyle Style) : View(Node, Node.Id);

public record NodeControl(NodeView View, SlotControl Header, IReadOnlyList<SlotControl> Slots, Func<IUpdates, IControl, IControl, IUpdates> Callback) 
    : Control<NodeView>(View.Node.Rect, View, Slots.Prepend(Header).ToList(), Callback)
{
    public StyledEllipse NodeShadow() => new(
        new ShapeStyle(View.Style.ShadowColor, PenStyle.Empty),
        new Ellipse(Measurements.RelativeRect.BottomCenter(), new Radius(View.Node.Rect.HalfWidth() * 1.3, View.Node.Rect.HalfWidth() * 0.3)));

    public StyledRect StyledShape() 
        => new(View.Style.ShapeStyle, Shape());

    public RoundedRect Shape() 
        => new(Measurements.RelativeRect, View.Style.Radius);

    public override ICanvas Draw(ICanvas canvas)
        => canvas.Draw(NodeShadow())
            .Draw(StyledShape());

    public override IEnumerable<IBehavior> GetDefaultBehaviors()
        => new[] { new DraggingBehavior(this) };
}