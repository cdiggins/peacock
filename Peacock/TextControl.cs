using System.Windows;
using System.Windows.Media;

namespace Peacock;

public record TextView(
    object Id,
    Rect Rect,
    string Text = "", 
    bool HasFocus = false, 
    int CaretPos = 0, 
    int CharOffset = 0, 
    int SelStart = 0, 
    int SelCount = 0) : IView
{
    public TextView AddText(string text) 
        => this with { Text = Text + text };
}

// TODO: handle delete, cut, copy, paste, highlight, navigate 
// TODO: draw the flashing caret ... at the correct moment in time. 

public record TextControl(TextView View, Func<IUpdates, IControl, IControl, IUpdates> Callback) 
    : Control<TextView>(View.Rect, View, Callback)
{
    public override IUpdates Process(IInputEvent input, IUpdates updates) 
        => input is KeyDownEvent keyDown 
            ? updates.UpdateControl(this, (control) => ((TextControl)control).AddText(keyDown.Args.Key.ToString())) 
            : updates;

    public TextControl AddText(string text)
        => this with { View = View.AddText(text) };

    public override ICanvas Draw(ICanvas canvas)
    {
        // TODO: draw the text
        // TODO: draw the caret at the correct position
        // TODO: make the caret flash 
        // TODO: highlight text

        return canvas
            .Draw(new StyledRect(
                new ShapeStyle(
                    new(Colors.Azure),
                    new(new(Colors.DarkGray), 0.5)),
                new(View.Rect)))
            .Draw(new StyledText(
                new TextStyle(
                    new BrushStyle(Colors.Black),
                    "Segoe UI",
                    10,
                    Alignment.LeftCenter),
                View.Rect,
                View.Text));
    }
}