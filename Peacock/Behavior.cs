namespace Peacock;

/// <summary>
/// A default implementation of IBehavior that does nothing, and is intended to be used
/// as a base class for other behaviors. 
/// </summary>
public record Behavior<TState>(IControl Control) 
    : IBehavior
    where TState : new()
{
    public TState State { get; init; } = new();

    public virtual ICanvas Draw(ICanvas canvas)
        => canvas;

    public virtual IUpdates Process(InputEvent input, IUpdates updates)
        => updates;

    public IUpdates UpdateState(IUpdates updates, Func<TState, TState> update)
        => updates.UpdateBehavior(this, x => x.WithState(update(x.State)));

    public Behavior<TState> WithState(TState state)
        => this with { State = state };
}