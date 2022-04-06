using System;
using System.Windows;
using Peacock;

namespace Emu.Controls;

public record SlotStyle(ShapeStyle ShapeStyle, TextStyle TextStyle, TextStyle SmallTextStyle, Radius Radius);

public record SlotView(Slot Slot, SlotStyle Style) : View(Slot, Slot.Id);

public record SlotControl(Measures Measures, SlotView View, SocketControl? Left, SocketControl? Right, Func<IUpdates, IControl, IControl, IUpdates> Callback) 
    : Control<SlotView>(Measures, View, ToChildren(Left, Right), Callback)
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
        => new(View.Style.TextStyle, Relative.ShrinkAndOffset(TextOffset()), View.Slot.Label);
    
    public StyledText StyledTypeText() 
        => new(View.Style.SmallTextStyle, Relative.Shrink(TextOffset()), View.Slot.Type);
    
    public RoundedRect Shape() 
        => new(Relative, View.Style.Radius);

    public StyledRect StyledShape()
        => new(View.Style.ShapeStyle, Shape());
    
    public Size TextOffset() 
        => new(View.Style.Radius.X * 1.5, 0);
}