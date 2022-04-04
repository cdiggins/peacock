namespace Peacock;

/// <summary>
/// A default implementation of IBehavior that does nothing, and is intended to be used
/// as a base class for other behaviors. 
/// </summary>
public record Behavior : IBehavior
{
    public virtual ICanvas Draw(IControl control, ICanvas canvas)
        => canvas;

    public virtual IUpdates ProcessInput(IControl control, InputEvent input, IUpdates updates)
        => updates;
}