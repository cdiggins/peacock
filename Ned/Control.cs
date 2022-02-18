using System;
using System.Collections.Generic;
using System.Linq;

namespace Ned;

public interface IControl
{
    public IView View { get; }
    public double ZOrder { get; }
    public IReadOnlyList<IControl> Children { get; }
    public IReadOnlyList<IBehavior> Behaviors { get; }
    public ICanvas Draw(ICanvas canvas);
    public (IControl, IUpdates) ProcessInput(IUpdates updates, InputEvent input);
    public IControl AddBehavior(IBehavior behavior);
    public IControl RemoveBehavior(IBehavior behavior);
    public IControl UpdateView(IView view);
}

/// <summary>
/// A generic immutable UI control. Instead of being changed, a new version is returned when processing input, 
/// or upon recieving new state. 
/// The control contains an immutable state object, list of child controls, and list of immutable behaviors. 
/// A behavior can perform additonal drawing operations on the canvas, and can process input events,
/// returning a new version of itself. 
/// A control requires explicit functions for:
///   * drawing to a canvas given a state object
///   * generating new children from the current state, and previous set of children (if applicable)
/// </summary>
public class Control<TView> : IControl
    where TView : class, IView
{
    /// <summary>
    /// Constructing a control requires a function for drawing to a canvas, given the current view state.
    /// A function for creating child controls, given a change to state. 
    /// The list of children if null, will be created from the state object. 
    /// Children can be explicitly provided, for example if they are transformed in response to changes in input. 
    /// Input is only processed by behaviors 
    /// </summary>
    public Control(
        TView state,
        double zorder = 0,
        Func<ICanvas, TView, ICanvas>? drawFunc = null,
        Func<TView, IEnumerable<IControl>>? childrenFunc = null,
        IEnumerable<IControl>? children = null,
        IEnumerable<IBehavior>? behaviors = null)
    {
        ZOrder = zorder;
        View = state;
        DrawFunc = drawFunc ?? DefaultDrawFunc;
        ChildrenFunc = childrenFunc ?? DefaultChildrenFunc;
        Children = children?.ToList() ?? ChildrenFunc(state).ToList();
        Behaviors = behaviors?.ToList() ?? new List<IBehavior>();
    }
    
    public static ICanvas DefaultDrawFunc(ICanvas canvas, TView _) => canvas;
    public static IEnumerable<IControl> DefaultChildrenFunc(TView _) => Array.Empty<IControl>();

    public TView View { get; }
    public double ZOrder { get; }
    public Func<TView, IEnumerable<IControl>> ChildrenFunc { get; }
    public Func<ICanvas, TView, ICanvas> DrawFunc { get; }
    public IReadOnlyList<IControl> Children { get; }
    public IReadOnlyList<IBehavior> Behaviors { get; }

    public Control<TView> UpdateView(TView view)
        => new(view, ZOrder, DrawFunc, ChildrenFunc, MergeChildren(ChildrenFunc(view)), Behaviors);

    public ICanvas Draw(ICanvas canvas)
    {
        var backgroundChildren = Children.Where(c => c.ZOrder < 0).OrderBy(c => c.ZOrder);
        var backgroundBehaviors = Behaviors.Where(c => c.ZOrder < 0).OrderBy(c => c.ZOrder);
        var foregroundChildren = Children.Where(c => c.ZOrder >= 0).OrderBy(c => c.ZOrder);
        var foregroundBehaviors = Behaviors.Where(c => c.ZOrder >= 0).OrderBy(c => c.ZOrder);

        canvas = backgroundBehaviors.Aggregate(canvas, (canvas, x) => x.Draw(this, canvas));
        canvas = backgroundChildren.Aggregate(canvas, (canvas, x) => x.Draw(canvas));
        
        canvas = DrawFunc(canvas, View);
        
        canvas = foregroundChildren.Aggregate(canvas, (canvas, x) => x.Draw(canvas));
        canvas = foregroundBehaviors.Aggregate(canvas, (canvas, x) => x.Draw(this, canvas));        
        
        return canvas;
    }

    public (IControl, IUpdates) ProcessInput(IUpdates updates, InputEvent input)
    {
        var newBehaviors = new List<IBehavior>();
        foreach (var b in Behaviors)
        {
            (updates, var newBehavior) = b.ProcessInput(this, updates, input);
            if (newBehavior != null)
                newBehaviors.Add(newBehavior);        
        }

        var newChildren = new List<IControl>();
        foreach (var c in Children)
        {
            (var control, updates) = c.ProcessInput(updates, input);
            if (control != null)
                newChildren.Add(control);   
        }

        return (new Control<TView>(View, ZOrder, DrawFunc, ChildrenFunc, newChildren, newBehaviors), updates);
    }
    
    public Control<TView> AddBehavior(IBehavior behavior)
        => new(View, ZOrder, DrawFunc, ChildrenFunc, Children, Behaviors.Add(behavior));

    public Control<TView> RemoveBehavior(IBehavior behavior)
        => new(View, ZOrder, DrawFunc, ChildrenFunc, Children, Behaviors.Remove(behavior));

    IControl IControl.AddBehavior(IBehavior behavior)
        => AddBehavior(behavior);

    IControl IControl.RemoveBehavior(IBehavior behavior)
        => RemoveBehavior(behavior);

    IView IControl.View => View;

    IControl IControl.UpdateView(IView view)
        => UpdateView((TView)view);

    public IReadOnlyList<IControl> MergeChildren(IEnumerable<IControl> newChildren)
    {
        var lookup = Children.ToDictionary(c => c.View.Id, c => c);
        return newChildren.Select(c => lookup.TryGetValue(c.View.Id, out var control) 
            ? control.UpdateView(c.View) : c).ToList();
    }
}
