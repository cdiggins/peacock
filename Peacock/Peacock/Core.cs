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
    IReadOnlyList<IComponent> Children { get; }
    IComponent FromModel(IDataSource model);
    IDataSource ToModel(IDataSource model);
    ICanvas Draw(ICanvas canvas);
    IComponent ProcessInput(IInputEvent input);
    IComponent With(ComponentState state, IEnumerable<IComponent>? children);
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

public abstract record Component<TComponent, TState> : IComponent
    where TState : ComponentState
    where TComponent : Component<TComponent, TState>    
{
    public abstract TComponent With(TState state, IEnumerable<IComponent>? children);

    IComponent IComponent.With(ComponentState state, IEnumerable<IComponent>? children)
        => With(State.Merge(state), children);

    protected Component(TState state, IEnumerable<IComponent>? children = null)
        => (State, Children) = (state, children?.ToArray() ?? Array.Empty<IComponent>());

    // The state can only be changed through the constructor (or the "with" function)
    public TState State { get; }

    // If the binding or funcs change, this does not affect the construction of the widget, so 
    public Binding<TState>? Binding { get; init; }
    public ComponentFuncs<TState>? Funcs { get; init; }

    // Just to make this compatible with IComponent
    ComponentState IComponent.State => State;

    public TState BindingFromModel(IDataSource source) 
        => Binding?.FromModel?.Invoke(source) ?? State;

    public IDataSource BindingToModel(IDataSource source, TState state) 
        => Binding?.ToModel?.Invoke(source, state) ?? source;

    public IComponent FromModel(IDataSource model)
        => With(BindingFromModel(model), Children?.Select(c => c.FromModel(model)));

    public IDataSource ToModel(IDataSource model)
        => Children.Aggregate(BindingToModel(model, State), (model, child) => child.ToModel(model));

    public virtual ICanvas Draw(ICanvas canvas)
        => Funcs?.OnDraw?.Invoke(State, canvas) ?? canvas;

    public virtual IComponent ProcessInput(IInputEvent input)
    {
        var newChildren = Children?.Select(c => c.ProcessInput(input));
        var newState = Funcs?.OnInput?.Invoke(State, input) ?? State;
        return With(newState, newChildren);
    }

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
