using System;
using System.Collections.Generic;
using System.Linq;

namespace Peacock;

public interface IComponentBase
{
    Guid Id { get; }
}

// A component has state, children, and wrappers 
public interface IComponent : IComponentBase
{
    IState State { get; }
    IReadOnlyList<IComponent> Children { get; }
    IReadOnlyList<IComponentWrapper> Wrappers { get; }
    ICanvas Draw(ICanvas canvas);
    (IComponent, IEventAggregator) ProcessInput(IInputEvent input, IEventAggregator aggregator);
    IComponent With(IState state, IReadOnlyList<IComponent> children, IReadOnlyList<IComponentWrapper> wrappers);
}

// A wrapper provides custom draw and/or input processing 
// If during input it returns a null component wrapper, it is stripped from the component 
public interface IComponentWrapper : IComponentBase
{
    ICanvas Draw(IComponent wrappedComponent, ICanvas canvas);
    (IComponent, IComponentWrapper?, IEventAggregator) ProcessInput(IComponent wrappedComponent, IInputEvent input, IEventAggregator aggregator);
}

public record ComponentBase(IState State): IComponentBase
{
    public Guid Id { get; init; } = Guid.NewGuid();
}

// Default implementation of IComponent
public abstract record Component : ComponentBase, IComponent
{
    public IReadOnlyList<IComponent> Children { get; init; } = new List<IComponent>();
    public IReadOnlyList<IComponentWrapper> Wrappers { get; init; } = new List<IComponentWrapper>();

    protected Component(IState state, IReadOnlyList<IComponent> children, IReadOnlyList<IComponentWrapper> wrappers)
        : base(state) => (Children, Wrappers) = (children, wrappers);

    public abstract IComponent With(IState state, IReadOnlyList<IComponent> children, IReadOnlyList<IComponentWrapper> wrappers);

    protected virtual ICanvas DrawImpl(ICanvas canvas)
        => canvas;

    public ICanvas Draw(ICanvas canvas)
    {
        if (this.NonZeroRect())
            canvas = canvas.SetRect(State.Rect);
        canvas = Wrappers.Aggregate(canvas, (_canvas, _comp) => _comp.Draw(this, _canvas));
        canvas = DrawImpl(canvas);
        canvas = Children.Aggregate(canvas, (_canvas, _comp) => _comp.Draw(_canvas));
        if (this.NonZeroRect())
            canvas = canvas.PopRect();
        return canvas;
    }

    protected virtual (IComponent, IEventAggregator) ProcessInputImpl(IComponent component, IInputEvent input, IEventAggregator aggregator)
        => (this, aggregator);

    public virtual (IComponent, IEventAggregator) ProcessInput(IInputEvent input, IEventAggregator aggregator)
    {
        // TODO: going from the local space to world space will be important for mouse coordinate. 
        // Similar to what happens in the canvas.
        // NOTE: if the component is not focused, then I'm not sure that any inputs should be handled.

        var r = this as IComponent;
        var newWrappers = new List<IComponentWrapper>();
        foreach (var wrapper in Wrappers)
        {
            (r, IComponentWrapper? tmp, aggregator) = wrapper.ProcessInput(r, input, aggregator);
            if (tmp != null)
                newWrappers.Add(tmp);
        }

        (r, aggregator) = ProcessInputImpl(this, input, aggregator);

        var newChildren = new List<IComponent>();
        foreach (var child in r.Children)
        {
            (IComponent newChild, aggregator) = child.ProcessInput(input, aggregator);
            newChildren.Add(newChild);
        }

        r = r.With(r.State, newChildren, newWrappers);

        return (r, aggregator);
    }
}


