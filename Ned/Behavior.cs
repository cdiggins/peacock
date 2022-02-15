using System;

namespace Ned;

public interface IBehavior
{
    public ICanvas Draw(IControl control, ICanvas canvas);
    public (IControl, IBehavior) ProcessInput(IControl control, InputEvent input);
}

public record Behavior<TState>(
    TState State, 
    Func<IControl, ICanvas, TState, ICanvas>? DrawFunc, 
    Func<IControl, InputEvent, TState, (IControl, TState)>? ProcessInputFunc) 
    : IBehavior
{
    public ICanvas Draw(IControl control, ICanvas canvas)
        => DrawFunc?.Invoke(control, canvas, State) ?? canvas;

    public (IControl, IBehavior) ProcessInput(IControl control, InputEvent input)
    {
        if (ProcessInputFunc != null)
        {
            var (newControl, newState) = ProcessInputFunc.Invoke(control, input, State);
            return (newControl, this with { State = newState });
        }
        return (control, this);
    }
}
