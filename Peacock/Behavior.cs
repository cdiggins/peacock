namespace Peacock;

/// <summary>
/// A default implementation of IBehavior that does nothing, and is intended to be used
/// as a base class for other behaviors. 
/// </summary>
public record Behavior(IControl Control) : IBehavior
{
    public virtual ICanvas Draw(ICanvas canvas)
        => canvas;

    public virtual IUpdates Process(InputEvent input, IUpdates updates)
        => updates;
}