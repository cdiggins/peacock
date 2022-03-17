﻿using System.Windows;
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
    public IView Apply(Func<IView, IView> func) => func(this);
    public TextView AddText(string text) => this with { Text = Text + text };
}

public record TextInputBehavior : Behavior
{
    public double ZOrder => 0;
    public ICanvas Draw(IControl control, ICanvas canvas) => canvas;

    public (IUpdates, IBehavior) ProcessInput(IControl control, IUpdates updates, InputEvent input)
    {
        if (input is KeyDownEvent keyDown)
        {
            updates = updates.AddUpdate(control.View.Id,
                view => view is TextView textView
                    ? textView.AddText(keyDown.Args.Key.ToString())
                    : view);
        }

        return (updates, this);
    }
}

// TODO: handle delete, cut, copy, paste, highlight, navigate 
// TODO: draw the flashing caret ... at the correct moment in time. 

public record TextControl : BaseControl<TextView>
{
    public TextControl(TextView view) : base(view) { }

    public static ICanvas Draw(ICanvas canvas, TextView view)
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
                new(view.Rect)))
            .Draw(new StyledText(
                new TextStyle(
                    new BrushStyle(Colors.Black),
                    "Segoe UI",
                    10,
                    Alignment.LeftCenter), 
                view.Rect,
                view.Text));
    }
}