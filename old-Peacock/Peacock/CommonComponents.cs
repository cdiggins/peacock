using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Peacock;

// Stores a typed state object 

public abstract record TypedComponent<TState> : Component
    where TState : IState
{
    protected TypedComponent(TState state, IReadOnlyList<IComponent>? children = null, IReadOnlyList<IComponentWrapper>? wrappers = null)
        : base(state, children ?? Array.Empty<IComponent>(), wrappers ?? Array.Empty<IComponentWrapper>())
        => State = state;

    public new TState State { get; init; }
}

// Drawsa background and border 

public record BackgroundComponent<TState> : TypedComponent<TState>
    where TState: StyledState
{
    public BackgroundComponent(TState state, IReadOnlyList<IComponent>? children = null, IReadOnlyList<IComponentWrapper>? wrappers = null)
        : base(state, children, wrappers)
    { }

    protected override ICanvas DrawImpl(ICanvas canvas)
    {
        var brush = canvas.CreateBrush(State.Style.BackgroundColor);
        var pen = canvas.CreatePen(State.Style.BorderColor, State.Style.BorderWidth);
        return canvas.DrawRect(brush, pen, State.Rect);
    }

    public override IComponent With(IState state, IReadOnlyList<IComponent> children, IReadOnlyList<IComponentWrapper> wrappers)
        => new BackgroundComponent<TState>((TState)state, children, wrappers);
}

// Stack component

public record StackState(StyledState State)
    : StyledState(State);

public record StackComponent(StackState state, IReadOnlyList<IComponent>? children = null, IReadOnlyList<IComponentWrapper>? wrappers = null)    
    : BackgroundComponent<StackState>(state, state.Rect.ArrangeStack(children ?? Array.Empty<IComponent>()).ToList(), wrappers)
{
    public override IComponent With(IState state, IReadOnlyList<IComponent> children, IReadOnlyList<IComponentWrapper> wrappers)
        => new StackComponent((StackState)state, children, wrappers);
}

// Button component

public record ButtonState(StyledState State, bool Down = false)
    : StyledState(State);

public record ButtonComponent(ButtonState state, IReadOnlyList<IComponent>? children = null, IReadOnlyList<IComponentWrapper>? wrappers = null)
    : BackgroundComponent<ButtonState>(state, children, wrappers )
{
    public override IComponent With(IState state, IReadOnlyList<IComponent> children, IReadOnlyList<IComponentWrapper> wrappers)
        => new ButtonComponent((ButtonState)state, children, wrappers);
}

// Grid component

public record GridComponent(StyledState State, IReadOnlyList<IComponent>? Children = null, IReadOnlyList<IComponentWrapper>? wrappers = null)
    : BackgroundComponent<StyledState>(State, Children?.ArrangeGrid(State.Rect).ToList())
{
    public override IComponent With(IState state, IReadOnlyList<IComponent> children, IReadOnlyList<IComponentWrapper> wrappers)
        => new GridComponent((StyledState)state, children, wrappers);
}

// Label component

public record LabelState(StyledState State, string Text)
    : StyledState(State);

public record LabelComponent(LabelState state, IReadOnlyList<IComponent>? children = null, IReadOnlyList<IComponentWrapper>? wrappers = null)
    : TypedComponent<LabelState>(state, children, wrappers)
{
    protected override ICanvas DrawImpl(ICanvas canvas)
    {
        var font = canvas.CreateFont(State.Style.FontFamily, State.Style.FontSize);
        var brush = canvas.CreateBrush(State.Style.FontColor);
        var size = canvas.MeasureString(font, brush, State.Text);
        var position = State.Rect.AlignedTopLeftOfSubarea(size, State.Style.Alignment);
        return canvas.DrawText(font, brush, position, State.Text);
    }

    public override IComponent With(IState state, IReadOnlyList<IComponent> children, IReadOnlyList<IComponentWrapper> wrappers)
        => new LabelComponent((LabelState)state, children, wrappers);
}

public record MouseOverDecorator(IState Original, Func<IState, double, IState> Transition, bool Hovering = false) 
    : IComponentWrapper
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTimeOffset Started { get; init; }

    public ICanvas Draw(IComponent component, ICanvas canvas)
         => canvas;

    public (IComponent, IComponentWrapper?, IEventAggregator)
        ProcessInput(IComponent wrappedComponent, IInputEvent input, IEventAggregator aggregator)
    {
        // TODO: This should be triggered on idle events as well so things can be animated  
        if (input is MouseMoveEvent mme)
        {
            if (Hovering)
            {
                if (wrappedComponent.State.Rect.Contains(mme.Point))
                    return (wrappedComponent.WithState(Transition(wrappedComponent.State, (DateTimeOffset.Now - Started).TotalSeconds)), this, aggregator);
                else
                    return (wrappedComponent.WithState(Original), this with { Hovering = false}, aggregator);
            }
            else
            {
                if (wrappedComponent.State.Rect.Contains(mme.Point))
                {
                    return (wrappedComponent.WithState(Transition(wrappedComponent.State, 0)),
                        this with { Hovering = true, Started = DateTimeOffset.Now }, aggregator);
                }
            }
        }
        return (wrappedComponent, this, aggregator);
    }
}

public record DraggableDecorator : IComponentWrapper
{
    public Guid Id { get; } = Guid.NewGuid();

    public bool Dragging { get; init; } = false;
    public Point StartPosition { get; init; }
    public Point CurrentPosition { get; init; }

    public ICanvas Draw(IComponent wrappedComponent, ICanvas canvas)
        => canvas;

    public (IComponent, IComponentWrapper?, IEventAggregator) 
        ProcessInput(IComponent wrappedComponent, IInputEvent input, IEventAggregator aggregator)
    {
        if (!Dragging)
        {
            if (input is MouseDownEvent mde)
            {
                return (
                    wrappedComponent,
                    this with { Dragging = true, StartPosition = mde.Point },
                    aggregator.AddEvent(this, new DragStartEvent(mde.Point)));
            }
        }
        else
        {
            if (input is MouseMoveEvent mme)
            {
                return (
                    wrappedComponent,
                    this with { CurrentPosition = mme.Point },
                    aggregator.AddEvent(this, new DragMoveEvent(mme.Point.Subtract(StartPosition))));
            }
            else if (input is MouseUpEvent mue)
            {
                return (
                    wrappedComponent,
                    this with { Dragging = false, CurrentPosition = mue.Point },
                    aggregator.AddEvent(this, new DragEndEvent()));
            }
        }

        return (wrappedComponent, this, aggregator);
    }
}


