using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Peacock;

public class Decorator : IComponentWrapper
{
    public Guid Id { get; } = Guid.NewGuid();

    public Decorator(Func<IComponent, ICanvas, ICanvas> func)
        => Func = func;

    Func<IComponent, ICanvas, ICanvas> Func { get; }

    public ICanvas Draw(IComponent wrappedComponent, ICanvas canvas)
        => Func(wrappedComponent, canvas);

    public (IComponent, IComponentWrapper?, IEventAggregator) ProcessInput(IComponent wrappedComponent, IInputEvent input, IEventAggregator handler)
        => (wrappedComponent, this, handler);
}

public class Behavior : IComponentWrapper
{
    public Guid Id { get; }

    public Behavior(Func<IComponent, IInputEvent, IEventAggregator, (IComponent, IComponentWrapper?, IEventAggregator)> func)
        => Func = func;

    Func<IComponent, IInputEvent, IEventAggregator, (IComponent, IComponentWrapper?, IEventAggregator)> Func { get; }

    public ICanvas Draw(IComponent wrappedComponent, ICanvas canvas)
        => canvas;

    public (IComponent, IComponentWrapper?, IEventAggregator) ProcessInput(IComponent wrappedComponent, IInputEvent input, IEventAggregator handler)
        => Func(wrappedComponent, input, handler);
}

public class OnInputBehavior<T> : IComponentWrapper
    where T: InputEvent
{
    public Guid Id { get; } = Guid.NewGuid();

    public OnInputBehavior(Func<IComponent, T, IComponent> func)
        => Func = func;

    Func<IComponent, T, IComponent> Func { get; }

    public ICanvas Draw(IComponent wrappedComponent, ICanvas canvas)
        => canvas;

    public (IComponent, IComponentWrapper?, IEventAggregator) ProcessInput(IComponent wrappedComponent, IInputEvent input, IEventAggregator handler)
    {
        var r = wrappedComponent;
        if (input is T typedInput)
        {
            r = Func(wrappedComponent, typedInput);
        }
        return (r, this, handler);
    }
}

public static class MoreExtensions
{
    public static Rect Shrink(this Rect r, Rect padding)
        => r.Shrink(padding.Left, padding.Top, padding.Right, padding.Bottom);

    public static Rect Shrink(this Rect r, double left, double top, double right, double bottom)
        => new Rect(r.Left + left, r.Top + top, r.Width - left - right, r.Height - top - bottom);

    public static Rect Shrink(this Rect r, double padding)
        => r.Shrink(padding, padding, padding, padding);

    public static ICanvas DrawCircle(this ICanvas canvas, IBrush? brush, IPen? pen, Point center, double radius)
        => canvas.DrawEllipse(brush, pen, center, radius, radius);

    public static ICanvas DrawEllipse(this ICanvas canvas, IPen pen, Point center, double radiusX, double radiusY)
        => canvas.DrawEllipse(null, pen, center, radiusX, radiusY);

    public static ICanvas DrawElipse(this ICanvas canvas, IBrush brush, Point center, double radiusX, double radiusY)
        => canvas.DrawEllipse(brush, null, center, radiusX, radiusY);

    public static ICanvas DrawRect(this ICanvas canvas, IPen pen, Rect rect)
        => canvas.DrawRect(null, pen, rect);

    public static ICanvas DrawRoundedRect(this ICanvas canvas, IPen pen, Rect rect, double radiusX, double radiusY)
        => canvas.DrawRoundedRect(null, pen, rect, radiusX, radiusY);

    public static ICanvas DrawRect(this ICanvas canvas, IBrush brush, Rect rect)
        => canvas.DrawRect(brush, null, rect);

    public static ICanvas DrawRoundedRect(this ICanvas canvas, IBrush brush, Rect rect, double radiusX, double radiusY)
        => canvas.DrawRoundedRect(brush, null, rect, radiusX, radiusY);

    public static IEnumerable<IComponent> Arrange(this IEnumerable<IComponent> components, IEnumerable<Rect> rects)
        => components.Zip(rects, (c, r) => c.WithRect(r));

    /*
    public static IWindow SetPos(this IWindow window, Point pos)
        => window.SetRect(new Rect(pos, window.Rect.Size));
    */

    public static Rect GetRow(this Rect r, int row, int rowCount)
        => new(r.Left, r.Top + r.Height * row / rowCount, r.Width, r.Height / rowCount);

    public static Rect GetColumn(this Rect r, int col, int colCount)
        => new(r.Left + r.Width * col / colCount, r.Top, r.Width / colCount, r.Bottom);

    public static Rect GetRowColumn(this Rect r, int row, int col, int rowCount, int colCount)
        => r.GetRow(row, rowCount).GetColumn(col, colCount);

    public static IEnumerable<Rect> GetGrid(this Rect r, int rowCount, int colCount)
        => Enumerable.Range(0, rowCount * colCount).Select(i => r.GetRowColumn(i / colCount, i % colCount, rowCount, colCount));

    public static IEnumerable<IComponent> ArrangeGrid(this IReadOnlyList<IComponent> components, Rect rect)
        => components.Arrange(rect.GetGrid(
            (int)Math.Ceiling(Math.Sqrt(components.Count)), 
            (int)Math.Ceiling(Math.Sqrt(components.Count))));

    public static IEnumerable<IComponent> ArrangeStack(this Rect rect, IEnumerable<IComponent> components, bool horizontalOrVertical = false, bool reverse = false)
        => components.Arrange(rect.ComputeStackLayout(components.Select(w => w.State.Rect).ToList(), horizontalOrVertical, reverse));

    public static IEnumerable<Rect> ComputeStackLayout(this Rect rect, IReadOnlyList<Rect> children, bool horizontalOrVertical = false, bool reverse = false)
        => horizontalOrVertical
            ? reverse
                ? rect.ComputeHorizontalLayoutRight(children)
                : rect.ComputeHorizontalLayoutLeft(children)
            : reverse
                ? rect.ComputeVerticalLayoutUp(children)
                : rect.ComputeVerticalLayoutDown(children);

    public static IEnumerable<Rect> ComputeVerticalLayoutDown(this Rect rect, IReadOnlyList<Rect> children)
    {
        var top = rect.Top;
        for (var i = 0; i < children.Count - 1; ++i)
            yield return new Rect(new Point(rect.Left, top), new Point(rect.Right, top += children[i].Height));
        yield return new Rect(new Point(rect.Left, top), new Point(rect.Right, rect.Bottom));
    }

    public static IEnumerable<Rect> ComputeHorizontalLayoutRight(this Rect rect, IReadOnlyList<Rect> children)
    {
        var left = rect.Left;
        for (var i = 0; i < children.Count - 1; ++i)
            yield return new Rect(new Point(left, rect.Top), new Point(left += children[i].Width, rect.Bottom));
        yield return new Rect(new Point(left, rect.Top), new Point(rect.Right, rect.Bottom));
    }

    public static IEnumerable<Rect> ComputeVerticalLayoutUp(this Rect rect, IReadOnlyList<Rect> children)
    {
        var bottom = rect.Bottom;
        for (var i = 0; i < children.Count - 1; ++i)
            yield return new Rect(new Point(rect.Left, bottom -= children[i].Height), new Size(rect.Width, children[i].Height));
        yield return new Rect(new Point(rect.Left, rect.Top), new Point(rect.Right, bottom));
    }

    public static IEnumerable<Rect> ComputeHorizontalLayoutLeft(this Rect rect, IReadOnlyList<Rect> children)
    {
        var right = rect.Right;
        for (var i = 0; i < children.Count - 1; ++i)
            yield return new Rect(new Point(right -= children[i].Width, rect.Bottom), new Size(children[i].Width, rect.Height));
        yield return new Rect(new Point(rect.Left, rect.Top), new Point(right, rect.Bottom));
    }

    public static Rect GetRect(this Window window)
        //=> new(window.Left, window.Top, window.Width, window.Height);
        => new(0, 0, window.Width, window.Height);

    public static WindowProps GetProps(this Window window)
        => new(window.GetRect(), window.Title, window.Cursor);

    public static Window ApplyRect(this Window window, Rect rect)
    {
        window.Left = rect.Left;
        window.Top = rect.Top;
        window.Width = rect.Width;
        window.Height = rect.Height;
        return window;
    }

    public static Window ApplyCursor(this Window window, Cursor cursor)
        => (window.Cursor = cursor, window).Item2;

    public static Window ApplyTitle(this Window window, string title)
        => (window.Title = title, window).Item2;

    public static Window ApplyProps(this Window window, WindowProps props)
        => window.ApplyRect(props.Rect).ApplyCursor(props.Cursor).ApplyTitle(props.Title);

    /*
    public static ICanvas SetWindowTitle(this ICanvas canvas, string text)
        => canvas.SetWindowProps(canvas.WindowProps with { Title = text });

    public static ICanvas SetWindowCursor(this ICanvas canvas, Cursor cursor)
       => canvas.SetWindowProps(canvas.WindowProps with { Cursor = cursor });

    public static ICanvas SetWindowRect(this ICanvas canvas, Rect rect)
       => canvas.SetWindowProps(canvas.WindowProps with { Rect = rect });
    */

    public static Rect Resize(this Rect rect, Size size)
        => new(rect.Location, size);

    public static Rect Move(this Rect rect, Point point)
        => new(point, rect.Size);

    public static Point Offset(this Point self, Point point)
        => new(self.X + point.X, self.Y + point.Y);

    public static Rect Offset(this Rect rect, Point point)
        => new(rect.Location.Offset(point), rect.Size);

    public static IComponent With(this IComponent self, IState state)
        => self.With(state, self.Children, self.Wrappers);

    public static IComponent With(this IComponent self, Func<IState, IState> func)
        => self.With(func(self.State));

    public static IComponent WithRect(this IComponent self, Rect rect)
        => self.With(self.State.WithRect(rect));

    public static T WithRect<T>(this T self, Rect rect)
        where T : ComponentState    
        => self with { Rect = rect };
        
    public static string ToDebugString(this IComponent self, string indent = "")
        => $"{indent}{self.GetType().Name} {self.State.Rect} [\n{string.Join(",\n", self.Children.Select(x => x.ToDebugString(indent + "  ")))}\n{indent}]";

    /*
    public static Component<TComponent, TState> OnMouseDown<TComponent, TState>(this Component<TComponent, TState> self, Action<MouseDownEvent> mde)
        => self with { Funcs = self.Funcs with { OnInput = (x, y) => mde(x,)} }
    */

    public static IComponent AddWrapper(this IComponent component, IComponentWrapper wrapper)
        => component.With(component.State, component.Children, component.Wrappers.Append(wrapper).ToList());

    public static IComponent OnInput<T>(this IComponent component, Func<IComponent, T, IComponent> f)
        where T : InputEvent
        => component.AddWrapper(new OnInputBehavior<T>(f));

    public static IComponent WithStyle(this IComponent component, Style style)
        => component.With(state => ((StyledState)state) with { Style = style });

    public static Style GetStyle(this IComponent component)
        => ((StyledState)component.State).Style;

    public static IComponent WithStyle(this IComponent component, Func<Style, Style> func)
        => component.WithStyle(func(component.GetStyle()));

    public static IComponent WithState(this IComponent component, Func<IState, IState> func)
        => component.WithState(func(component.State));

    public static IComponent WithState(this IComponent component, IState state)
        => component.With(state, component.Children, component.Wrappers);

    public static Point Subtract(this Point self, Point point)
        => new(self.X - point.X, self.Y - point.Y);

    public static IComponent AddDraggable(this IComponent self)
        => self.AddWrapper(new DraggableDecorator());

    public static IEnumerable<T> GetEvents<T>(this IEventAggregator e)
        where T : IComponentEvent
        => e.Events.Select(x => x.Item2).OfType<T>();

    public static Point GetScreenPosition(this MouseEventArgs args, Window window)
        => window.PointToScreen(args.GetPosition(window));

    public static double HalfHeight(this Rect rect)
        => rect.Top + rect.Size.HalfHeight();

    public static double HalfHeight(this Size size)
        => size.Height / 2.0;

    public static double HalfWidth(this Rect rect)
        => rect.Top + rect.Size.HalfWidth();

    public static double HalfWidth(this Size size)
        => size.Width / 2.0;

    public static Point Center(this Rect r)
        => new(r.Left + r.HalfWidth(), r.Top + r.HalfHeight());

    public static Size Half(this Size size)
        => new(size.HalfWidth(), size.HalfHeight());

    public static Point Subtract(this Point p, Size size)
        => new(p.X - size.Width, p.Y - size.Width);

    public static Point CenterTopLeftOfSubarea(this Rect rect, Size size)
        => rect.Center().Subtract(size.Half());

    public static Point AlignedTopLeftOfSubarea(this Rect rect, Size size, Alignment alignment)
    {
        var center = rect.CenterTopLeftOfSubarea(size);
        var right = rect.Right - size.Width;
        var bottom = rect.Bottom - size.Height;
        var left = rect.Left;
        var top = rect.Top;

        switch (alignment)
        {
            case Alignment.TopCenter:
                return new(center.X, top);
            case Alignment.TopRight:
                return new(right, top);
            case Alignment.CenterLeft:
                return new(left, center.Y);
            case Alignment.CenterCenter:
                return center;
            case Alignment.CenterRight:
                return new(right, center.Y);
            case Alignment.BottomLeft:
                return new(left, bottom);
            case Alignment.BottomCenter:
                return new(center.X, bottom);
            case Alignment.BottomRight:
                return new(right, bottom);           
            case Alignment.None:
            case Alignment.TopLeft:
            default:
                return rect.TopLeft;
        }
    }

    public static bool NonZero(this Rect self)
        => self.Width > 0 && self.Height > 0;

    public static bool NonZeroRect(this IComponent self)
        => self.State.Rect.NonZero();
}
    