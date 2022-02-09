using System;
using System.Collections.Generic;
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
    public Rect Initial { get; init; } = Constants.Default;
    public Rect Actual { get; init; } = Constants.Default;
}

public interface IState
{
    bool Enabled { get; }
    bool Active { get; }
    bool Visible { get; }
    bool Hovered { get; }
    bool Rendered { get; }
    Rect Rect { get; }

    IState WithEnabled(bool enabled);
    IState WithActive(bool active);
    IState WithVisible(bool visible);
    IState WithHovered(bool hovered);
    IState WithRendered(bool rendered);
    IState WithRect(Rect rect);
}

public record ComponentState : IState
{
    public bool Enabled { get; init; }
    public bool Active { get; init; }
    public bool Visible { get; init; }
    public bool Hovered { get; init; }
    public bool Rendered { get; init; }
    public Rect Rect { get; init; }

    public IState WithEnabled(bool enabled) => this with { Enabled = enabled };
    public IState WithActive(bool active) => this with { Active = active };
    public IState WithVisible(bool visible) => this with { Visible = visible };
    public IState WithHovered(bool hovered) => this with { Hovered = hovered };
    public IState WithRendered(bool rendered) => this with { Rendered = rendered };
    public IState WithRect(Rect rect) => this with { Rect = rect };
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

    public StyledState WithStyle(Style style) => this with { Style = style };
    public StyledState WithDefaultStyle(Style style) => this with { DefaultStyle = style };
}
