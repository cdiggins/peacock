namespace Peacock;

public interface IBehavior
{
    public double ZOrder { get; }
    public ICanvas Draw(IControl control, ICanvas canvas);
    public (IUpdates, IBehavior) ProcessInput(IControl control, IUpdates updates, InputEvent input);
}

public interface IUpdates
{
    IUpdates AddUpdate(Guid id, Func<IView, IView> func);
    IReadOnlyDictionary<Guid, List<Func<IView, IView>>> Lookup { get; }
}

public record Behavior<TState>(
    double ZOrder,
    TState State, 
    Func<IControl, ICanvas, TState, ICanvas>? DrawFunc, 
    Func<IControl, IUpdates, InputEvent, TState, (IUpdates, TState)>? ProcessInputFunc) 
    : IBehavior
{
    public ICanvas Draw(IControl control, ICanvas canvas)
        => DrawFunc?.Invoke(control, canvas, State) ?? canvas;

    public (IUpdates, IBehavior) ProcessInput(IControl control, IUpdates updates, InputEvent input)
    {
        if (ProcessInputFunc != null)
        {
            (updates, var newState) = ProcessInputFunc.Invoke(control, updates, input, State);
            return (updates, this with { State = newState });
        }
        return (updates, this);
    }
}

public static class BehaviorExensions
{
    public static IUpdates AddUpdate<T>(this IUpdates self, Control<T> control, Func<T, T> f)
        where T : class, IView 
        => self.AddUpdate(control.View.Id, view => f((T)view));

    public static T ApplyUpdates<T>(this T self, IUpdates updates)
        where T : IView
        => (T)self.Apply(view =>
        {
            if (updates.Lookup.TryGetValue(view.Id, out var funcList))
            {
                foreach (var f in funcList)
                    view = f(view);
            }
            return (T)view;
        });
}

