using System.Windows;
using System.Windows.Media;

namespace Peacock;

public record TextView(
    object Id,
    string Text = "", 
    bool HasFocus = false, 
    int CaretPos = 0, 
    int CharOffset = 0, 
    int SelStart = 0, 
    int SelCount = 0) : IView;

// TODO: handle delete, cut, copy, paste, highlight, navigate 
// TODO: draw the flashing caret ... at the correct moment in time. 

public record TextControl(TextView View, Measures Measures, Func<IUpdates, IControl, IControl, IUpdates> Callback) 
    : Control<TextView>(Measures, View, Array.Empty<IControl>(), Callback)
{
    public override IUpdates Process(IInputEvent input, IUpdates updates) 
    {
        if (input is KeyDownEvent keyDown)
        {
            return updates.UpdateControl(this, (control) => ((TextControl)control).ProcessKey(keyDown));
        }
        else if (input is TextInputEvent textInput)
        {
            return updates.UpdateControl(this, (control) => ((TextControl)control).AddText(textInput.Args.Text));
        }
        else
        {
            return updates;
        }
    }

    public TextControl UpdateCaretPos(int newPos)
    {
        if (newPos < 0) newPos = 0;
        if (newPos > View.Text.Length) newPos = View.Text.Length;
        return this with { View = View with { CaretPos = newPos } };
    }
    
    public TextControl ProcessKey(KeyDownEvent keyDown)
    {
        if (keyDown.Args.Key == System.Windows.Input.Key.Left)
        {
            return UpdateCaretPos(View.CaretPos - 1); 
        }
        if (keyDown.Args.Key == System.Windows.Input.Key.Left)
        {
            return UpdateCaretPos(View.CaretPos - 1);
        }
        return this;
    }

    public TextControl AddText(string text)
        => this with { View = View with { Text = this.View.Text + text } };

    public override ICanvas Draw(ICanvas canvas)
    {
        // TODO: draw the text
        // TODO: draw the caret at the correct position
        // TODO: make the caret flash 
        // TODO: highlight text
        // TODO: move caret 
        // TODO: support highlighting the whole thing
        // TODO: support text left-right scrolling

        return canvas
            .Draw(new StyledRect(
                new ShapeStyle(
                    new(Colors.Azure),
                    new(new(Colors.DarkGray), 0.5)),
                new(Measures.AbsoluteRect)))
            .Draw(new StyledText(
                new TextStyle(
                    new BrushStyle(Colors.Black),
                    "Segoe UI",
                    FontWeight.Normal,
                    10,
                    Alignment.LeftCenter),
                Measures.AbsoluteRect,
                View.Text));
    }
}