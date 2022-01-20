using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Peacock;

// This is an abstraction for representing the source data model that are bound to components. 
public interface IDataSource
{ }

// A helper function to allow a user defined value type
public class DataSource<T> : IDataSource
{
    public DataSource(T data)
        => Data = data;

    public T Data { get; set; }
    
    public void AddBinding<TComponent, TProps>(TComponent component, Func<T, TProps> fromModelFunc, Func<T, TProps, T>? toModelFunc = null)
        => throw new NotImplementedException();
}

// This identifies type that contain input event data
// This could be generated from the Window system or from parent component 
public interface IInputEvent
{ }

// This an abstraction representing a component. 
public interface IComponent
{
    ComponentState State { get; }
    IComponent FromModel(IDataSource model);
    IDataSource ToModel(IDataSource model);
    ICanvas Draw(ICanvas canvas);
    IComponent OnInput(IInputEvent input);
    IComponent With(ComponentState state);
    IComponent With(Func<ComponentState, ComponentState> state);
}

// This an abstraction representing a component. 
public interface IComponent<TState>
    : IComponent
    where TState : ComponentState
{
    TState State { get; }
    IComponent<TState> FromModel(IDataSource model);
    IDataSource ToModel(IDataSource model);
    ICanvas Draw(ICanvas canvas);
    IComponent<TState> OnInput(IInputEvent input);
    IComponent<TState> With(TState state);
    IComponent<TState> With(Func<TState, TState> state);
}

// Represent one or two way binding from an arbitrary data-source to a state.
public record Binding<TState>
{
    public Func<IDataSource, TState>? FromModel { get; init; }
    public Func<IDataSource, TState, IDataSource>? ToModel { get; init; }
}

public record ComponentFuncs<TState>
{
    public Func<TState, ICanvas, ICanvas>? OnDraw { get; init; }
    public Func<TState, IInputEvent, TState>? OnInput { get; init; }
}

public interface IFactory<TComponent, TState>
    where TComponent : IComponent<TState>
    where TState : ComponentState
{
    TComponent With(Func<TState, TState> func);
}

public record Component<TComponent, TState>
    : IComponent<TState>
    where TState : ComponentState
    where TComponent : Component<TComponent, TState>    
{
    protected Component(TState state, IEnumerable<IComponent>? children = null)
        => (State, Children) = (state, children?.ToArray() ?? Array.Empty<IComponent>());

    // Optimization, find the constructor in advance.
    // TODO: don't guess at the constructor
    private readonly static Func<TComponent, TState, TComponent> Factory
        = (comp, state) => (TComponent)typeof(TComponent).GetConstructors()[1].Invoke(new object[] { comp, state });

    // The state can only be changed through the constructor (or the "with" function)
    public TState State { get; }

    // If the binding or funcs change, this does not affect the construction of the widget. 
    public Binding<TState>? Binding { get; init; }
    public ComponentFuncs<TState>? Funcs { get; init; }

    ComponentState IComponent.State => State;

    // TODO: Apply to children
    public IComponent<TState> FromModel(IDataSource model)
        => Binding?.FromModel != null
            ? With(Binding.FromModel.Invoke(model))
            : this;

    // TODO: Apply to children
    public IDataSource ToModel(IDataSource model)
        => Binding?.ToModel != null
            ? Binding.ToModel.Invoke(model, State)
            : model;

    // TODO: Apply to children
    public ICanvas Draw(ICanvas canvas)
        => Funcs?.OnDraw != null
            ? Funcs.OnDraw(State, canvas)
            : canvas;

    // TODO: Apply to children
    public IComponent<TState> OnInput(IInputEvent input)
        => Funcs?.OnInput != null
            ? With(Funcs.OnInput.Invoke(State, input))
            : this;

    public TComponent With(TState state)
        => Factory.Invoke((TComponent)this, state) with { Binding = Binding, Funcs = Funcs };

    public IComponent<TState> With(Func<TState, TState> func)
        => With(func(State));

    IComponent<TState> IComponent<TState>.With(TState state)
        => With(state);

    public IComponent With(Func<ComponentState, ComponentState> func)
        => With((TState state) => (TState)func(State));

    // TODO: if I only get the component state part, I still want to update things, but only the things I can update
    // Basically this needs a "merge" function
    public IComponent With(ComponentState state)
        => With((TState)state);

    IComponent IComponent.FromModel(IDataSource model)
        => FromModel(model);

    IComponent IComponent.OnInput(IInputEvent input)
        => OnInput(input);

    public IReadOnlyList<IComponent> Children { get; init; } 
        = Array.Empty<IComponent>();
}

public enum Alignment
{
    None,
    TopLeft,
    TopCenter,
    TopRight,
    CenterLeft,
    CenterCenter,
    CenterRight,
    BottomLeft,
    BottomCenter,
    BottomRight,
}

public record Style
{
    public Color BackgroundColor { get; init; } = Colors.Beige;
    public double BorderWidth { get; init; } = 1;
    public Color BorderColor { get; init; } = Colors.Blue;

    public string FontFamily { get; init; } = "Arial";
    public double FontSize { get; init; } = 12;
    public Color FontColor { get; init; } = Colors.Crimson;

    public Rect Padding { get; init; } = new Rect(5, 5, 5, 5);
    public Rect Margin { get; init; } = new Rect(5, 5, 5, 5);

    public Alignment Alignment { get; init; } = Alignment.TopLeft;
}

public static class Constants
{
    public static Rect Max = new(double.MinValue, double.MinValue, double.MaxValue, double.MaxValue);
    public static Rect Default = new(0, 0, 200, 50);
}

public record Dimensions
{
    public Rect Minimum { get; init; } = Rect.Empty;
    public Rect Maximum { get; init; } = Constants.Max;
    public Rect Preferred { get; init; } = Constants.Default;
    public Rect Actual { get; init; } = Constants.Default;
}

public record ComponentState
{
    public Guid Id { get; init; } 
        = Guid.NewGuid();

    public bool Enabled { get; init; }
    public bool Active { get; init; }
    public bool Visible { get; init; }
    public bool Hovered { get; init; }
    public bool Rendered { get; init; }

    public Dimensions Dimensions { get; init; } 
        = new Dimensions();
}

public record StyledState
    : ComponentState
{
    public Style Style { get; init; }
    public Style DefaultStyle { get; init; }
    public Style? EnabledStyle { get; init; }
    public Style? DisabledStyle { get; init; }
    public Style? HoveredStyle { get; init; }

    public StyledState(Style style)
        => (Style, DefaultStyle) = (style, style);
}

public record ButtonState(StyledState State, bool Down = false) 
    : StyledState(State);

public record LabelState(StyledState State, string Text) 
    : StyledState(State);

public record StackState(StyledState State)
    : StyledState(State);

// Here is the problem: whenever the rect changes, the stack component must update its children. 
// This does not work if the state is just hanging out and can be changed any time. 
// What has to happen is this constructor has to be recalled, with a new rect. 
// So what it means is that "With" does not work.

// States must support being nested.

// Changes to state must trigger the components constructor so that the right thing happens. 

// This could be forced through the "New" 

public record StackComponent(StackState State, IReadOnlyList<IComponent> Components)
    : Component<StackComponent, StackState>(State, State.Dimensions.Actual.ComputeStackLayout(Components));

public record ButtonComponent(ButtonState State, IComponent Child)
    : Component<ButtonComponent, ButtonState>(State, Child);

public record LabelComponent : Component<LabelComponent, LabelState>
{
    public LabelComponent(LabelState State)
        : base(State, Array.Empty<IComponent>())
    { }
}
    

// The idea of a component factory allows themes, and such things. 

public class ComponentFactory
{
    public Style DefaultStyle => new();
    public StyledState DefaultStyledState => new(DefaultStyle);

    public LabelComponent Label(string text)
        => new (new LabelState(DefaultStyledState, text).WithPreferredRect(new(0, 0, 100, 25)));

    public ButtonComponent Button(string text)
        => new (new(DefaultStyledState), Label(text));

    public StackComponent Stack(Rect Rect, params IComponent[] children)
        => new (new StackState(DefaultStyledState).WithRect(Rect), children);

    public StackComponent Stack(params IComponent[] children)
        => new (new StackState(DefaultStyledState), children);

    public LabelComponent TitleBar(string text)
        => Label(text).With<LabelComponent, LabelState>(state => state.WithPreferredRect(new(0,0,100,35)));

    public StackComponent Window(double width, double height, string text, IComponent content) 
        => Stack(new(0,0, width, height), new[] { TitleBar(text), content });
}
