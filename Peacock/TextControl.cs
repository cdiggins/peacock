using System.Windows;
using System.Windows.Media;

namespace Peacock;

public record TextView(
    Rect Rect,
    string Text = "", 
    bool HasFocus = false, 
    int CaretPos = 0, 
    int CharOffset = 0, 
    int SelStart = 0, 
    int SelCount = 0) : IView
{
    public Guid Id { get; }

    public TextView AddText(string text) 
        => this with { Text = Text + text };
}

// TODO: handle delete, cut, copy, paste, highlight, navigate 
// TODO: draw the flashing caret ... at the correct moment in time. 

public record TextControl(TextView View) : Control<TextView>(View)
{
    public override IView Process(IInputEvent input, IUpdates updates) 
        => input is KeyDownEvent keyDown ? View.AddText(keyDown.Args.Key.ToString()) : View;

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