using System;
using System.Windows;
using Peacock;

namespace Emu.Controls;

public record SlotStyle(ShapeStyle ShapeStyle, TextStyle TextStyle, TextStyle SmallTextStyle, Radius Radius);

public record SlotView(Slot Slot, SlotStyle Style) : View(Slot, Slot.Id);

public record SlotControl(Rect Rect, SlotView View, SocketControl? Left, SocketControl? Right, Func<IUpdates, IControl, IControl, IUpdates> Callback) 
    : Control<SlotView>(Rect, View, ToChildren(Left, Right), Callback)
{
    public override ICanvas Draw(ICanvas canvas) 
        => View.Slot.IsHeader 
            ? canvas         
                .Draw(StyledShape())
                .Draw(StyledText())
            : canvas
                .Draw(StyledShape())
                .Draw(StyledText())
                .Draw(StyledTypeText());

    public StyledText StyledText() 
        => new(View.Style.TextStyle, Measurements.RelativeRect.ShrinkAndOffset(TextOffset()), View.Slot.Label);
    
    public StyledText StyledTypeText() 
        => new(View.Style.SmallTextStyle, Measurements.RelativeRect.Shrink(TextOffset()), View.Slot.Type);
    
    public RoundedRect Shape() 
        => new(Measurements.RelativeRect, View.Style.Radius);

    public StyledRect StyledShape()
        => new(View.Style.ShapeStyle, Shape());
    
    public Size TextOffset() 
        => new(View.Style.Radius.X * 1.5, 0);
}