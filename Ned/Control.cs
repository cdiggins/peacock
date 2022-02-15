using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;

namespace Ned;

public interface IControl
{
    public IReadOnlyList<IControl> Children { get; }
    public IReadOnlyList<IBehavior> Behaviors { get; }
    public ICanvas Draw(ICanvas canvas);
    public IControl ProcessInput(InputEvent input);
    public IControl AddBehavior(IBehavior behavior);
    public IControl RemoveBehavior(IBehavior behavior);
}

// When a new state is provided, this has an impact on the children. 
// This could be stored in the control ... otherwise we end up in a tough situiation

public record Control<TState> : IControl
{
    public Control(TState state, Func<ICanvas, TState, ICanvas> drawFunc, IEnumerable<IControl>? children = null)
        => (State, DrawFunc, Children) = (state, drawFunc, children?.ToList() ?? new List<IControl>());
    
    public TState State { get; init; }
    public Func<ICanvas, TState, ICanvas> DrawFunc { get; init; }
    public IReadOnlyList<IControl> Children { get; init; }
    public IReadOnlyList<IBehavior> Behaviors { get; init; } = Array.Empty<IBehavior>();

    public ICanvas Draw(ICanvas canvas)
        => Children.Aggregate(
            DrawFunc(
                Behaviors.Aggregate(
                    canvas, (canvas, b) => b.Draw(this, canvas)),
                State),
                (canvas, c) => c.Draw(canvas)
                );

    public IControl ProcessInput(InputEvent input)
    {
        var newBehaviors = new List<IBehavior>();
        var newControl = this as IControl;
        foreach (var b in Behaviors)
        {
            (newControl, var newBehavior) = b.ProcessInput(newControl, input);
            if (newBehavior != null)
                newBehaviors.Add(newBehavior);        
        }
        var result = (Control<TState>)newControl;
        return result with
        {
            Children = result.Children.Select(c => c.ProcessInput(input)).WhereNotNull().ToList(),
            Behaviors = newBehaviors
        };
    }
    
    public IControl AddBehavior(IBehavior behavior)
        => this with { Behaviors = Behaviors.Add(behavior) };

    public IControl RemoveBehavior(IBehavior behavior)
        => this with { Behaviors = Behaviors.Remove(behavior) };
}
