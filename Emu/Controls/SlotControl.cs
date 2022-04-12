using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Emu.Behaviors;
using Peacock;

namespace Emu.Controls;

public record SlotStyle(ShapeStyle ShapeStyle, TextStyle TextStyle, TextStyle SmallTextStyle, Radius Radius, double TextOffset, double TabRadius);

public record SlotView(Slot Slot, SlotStyle Style) : View(Slot, Slot.Id);

public record SlotControl(Measures Measures, SlotView View, SocketControl? Left, SocketControl? Right, Func<IUpdates, IControl, IControl, IUpdates> Callback) 
    : Control<SlotView>(Measures, View, ToChildren(Left, Right), Callback)
{
    public override ICanvas Draw(ICanvas canvas)
        => View.Slot.Edit
            ? DrawEditBox(canvas) 
            : canvas.Draw(StyledText());

    public ICanvas DrawEditBox(ICanvas canvas)
        => canvas.Draw(new StyledRect(new ShapeStyle(Colors.LightGray, Colors.Transparent), EditBoxRect))
        .Draw(new StyledText(View.Style.TextStyle with { BrushStyle = Colors.Black }, EditBoxRect, View.Slot.Label));

    public ICanvas DrawTabs(ICanvas canvas)
    {       
        var tabSize = View.Style.TabRadius;
        var leftRect = Client.LeftCenter().Subtract(new Size(0, tabSize/2)).ToRect(new(tabSize/2, tabSize));
        var rightRect = Client.RightCenter().Subtract(new Size(0, tabSize/2)).ToRect(new(tabSize/2, tabSize));
        var leftTab = new StyledRect(View.Style.ShapeStyle, new (leftRect, View.Style.Radius));
        var rightTab = new StyledRect(View.Style.ShapeStyle, new (rightRect, View.Style.Radius));
        if (View.Slot.Left != null)
            canvas = canvas.Draw(leftTab);
        if (View.Slot.Right != null)
            canvas = canvas.Draw(rightTab);
        return canvas;
    }

    public Rect EditBoxRect
        => TextBoxRect.ShrinkFromCenter(new(2, 2));

    public Rect TextBoxRect
        => Client.Offset(TextOffset()).Shrink(TextOffset()).Shrink(TextOffset());

    public StyledText StyledText() 
        => new(View.Style.TextStyle, TextBoxRect, View.Slot.Label);
        
    public Size TextOffset() 
        => new(View.Style.TextOffset, 0);

    public override IUpdates Process(IInputEvent input, IUpdates updates)
    {
        if (input is TextInputEvent textInput)
        {
            return updates.UpdateModel(View.Slot, slot => 
                slot with { Label = slot.Label + textInput.Args.Text });
        }
        return base.Process(input, updates);
    }

    // Uncomment for certain types of behaviors
    //    public override IEnumerable<IBehavior> GetDefaultBehaviors()
    //        => new IBehavior[] { new ShowTabBehavior(this) };
}