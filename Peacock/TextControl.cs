using System.Windows;
using System.Windows.Media;

namespace Peacock;

public record TextEditState(
    int PrevCaretPos,
    int CaretPos,
    int CharOffset,
    int SelStart,
    int SelCount,
    bool InsertMode)
{
    public TextEditState()
        : this(0, 0, 0, 0, 0, true) 
    { } 
}

public record TextEditBehavior(object? ControlId) : Behavior<TextEditState>(ControlId)
{
    public int GetValidPos(TextControl control, int pos)
        => Math.Min(Math.Max(pos, 0), control.View.Text.Length);    

    public override IUpdates Process(IControl control, InputEvent input, IUpdates updates)
    {
        if (control is TextControl textControl)
        {
            if (textControl.Absolute.Contains(input.MouseStatus.Location))
            {
                if (input is KeyDownEvent keyDown)
                {
                    return UpdateState(updates, state => ProcessKeyDown(textControl, state, keyDown));
                }
                else if (input is TextInputEvent textInput)
                {
                    var insertedText = textInput.Args.Text;

                    if (IsTextSelected(textControl))
                    {
                        // Remove it
                        // Update the selection
                        // That selected 
                    }

                    // TODO: assure that the cursor is visible.
                    var oldText = textControl.View.Text;
                    var caretPos = GetValidPos(textControl, State.CaretPos);

                    var beforeCaretText = oldText.Substring(0, caretPos);
                    var afterCaretText = oldText.Substring(caretPos);
                    var newText = beforeCaretText + insertedText + afterCaretText;
                    var newCaretPos = beforeCaretText.Length + insertedText.Length;

                    // Move the caret to after the new text
                    updates = UpdateState(updates, state => state with { PrevCaretPos = State.CaretPos, CaretPos = newCaretPos });

                    // Update the text on the control (which will propagate back to model)
                    updates = textControl.UpdateView(updates, view => view with { Text = newText }); 
                }
            }
        }
        return base.Process(control, input, updates);
    }

    private TextEditState ProcessKeyDown(TextControl control, TextEditState state, KeyDownEvent keyDown)
    {
        // TODO: handle selection
        // TODO: handle tab
        // TODO: figure out 
        if (keyDown.Args.Key == System.Windows.Input.Key.Right)
            return state with { PrevCaretPos = state.CaretPos, CaretPos = GetValidPos(control, state.CaretPos + 1) };
        if (keyDown.Args.Key == System.Windows.Input.Key.Left)
            return state with { PrevCaretPos = state.CaretPos, CaretPos = GetValidPos(control, state.CaretPos - 1) };
        return state;
    }

    public bool IsTextSelected(TextControl control)
        => State.SelCount > 0 && State.SelStart > 0 && State.SelStart < control.View.Text.Length;

    public override ICanvas PostDraw(ICanvas canvas, IControl control)
    {
        // TODO: draw the caret. 
        return base.PostDraw(canvas, control);
    }
}

public record TextView(object Id, string Text = "") : IView;

public record TextControl(TextView View, Measures Measures, Func<IUpdates, IControl, IControl, IUpdates> Callback) 
    : Control<TextView>(Measures, View, Array.Empty<IControl>(), Callback)
{    
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
        // TODO: handle delete, cut, copy, paste, highlight, navigate 
        // TODO: draw the flashing caret ... at the correct moment in time. 
        // TODO: Spacebar
        // TODO: Tab
        // TODO: Backspace
        // TODO: Home/End/Delete/Insert keys
        // TODO: s  Arrow keys
        // Control key combinations, including Ctrl+V

        // TODO: how do I control what part of the text is visible? I have to make the caret position visible. 
        // I can scroll to the left ... or to the right. Now with long text if I move to the right, it shifts the text to the left, 
        // and vice versa. In a pure functional way ... this means I need to know where was I last, if I am shifted left ... 
        // how do I decide this ... well partly what direction I am moving ... otherwise things could go wrong. I need to minimize
        // the amount of movement to make things visible. Knowing the previous caret position would be sufficient 

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