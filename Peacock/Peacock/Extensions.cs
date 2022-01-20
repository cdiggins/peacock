using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Peacock;

public static class MoreExtensions
{
    public static Rect Subdivide(this Rect r, int i, int cnt)
    {
        Debug.Assert(r.Width >= 0);
        Debug.Assert(r.Height >= 0);
        Debug.Assert(i < cnt);
        var side = (int)Math.Ceiling(Math.Sqrt(cnt));
        if (side <= 0)
            return Rect.Empty;
        var row = i / side;
        var col = i % side;
        var width = r.Width / side;
        var height = r.Height / side;
        Debug.Assert(width <= r.Width);
        Debug.Assert(height <= r.Height);
        var left = r.Width * col / side + r.Left;
        var top = r.Height * row / side + r.Top;
        Debug.Assert(left >= 0);
        Debug.Assert(top >= 0);
        Debug.Assert(left + width <= r.Right);
        Debug.Assert(top + height <= r.Bottom);
        return new Rect(left, top, width, height);
    }

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

    /*
    public static IWindow SetPos(this IWindow window, Point pos)
        => window.SetRect(new Rect(pos, window.Rect.Size));
    */

    public static IEnumerable<IComponent> ComputeStackLayout(this Rect rect, IEnumerable<IComponent> components, bool horizontalOrVertical = false, bool reverse = false)
        => components.Zip(rect.ComputeStackLayout(
            components.Select(w => w.PreferredRect()).ToList(), horizontalOrVertical, reverse),
            (w, r) => w.WithRect(r));

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
        => new(window.Left, window.Top, window.Width, window.Height);

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

    public static ICanvas SetWindowTitle(this ICanvas canvas, string text)
        => canvas.SetWindowProps(canvas.WindowProps with { Title = text });

    public static ICanvas SetWindowCursor(this ICanvas canvas, Cursor cursor)
       => canvas.SetWindowProps(canvas.WindowProps with { Cursor = cursor });

    public static ICanvas SetWindowRect(this ICanvas canvas, Rect rect)
       => canvas.SetWindowProps(canvas.WindowProps with { Rect = rect });

    public static Rect Resize(this Rect rect, Size size)
        => new(rect.Location, size);

    public static Point Offset(this Point self, Point point)
        => new(self.X + point.X, self.Y + point.Y);

    public static Rect Offset(this Rect rect, Point point)
        => new(rect.Location.Offset(point), rect.Size);

    public static Dimensions Resize(this Dimensions dimensions, Size size)
        => dimensions.WithRect(dimensions.Actual.Resize(size));

    public static Dimensions Offset(this Dimensions dimensions, Point point)
        => dimensions.WithRect(dimensions.Actual.Offset(point));

    public static Dimensions WithRect(this Dimensions dimensions, Rect rect)
        => dimensions with { Actual = rect };

    public static Dimensions WithPreferredRect(this Dimensions dimensions, Rect rect)
        => dimensions with { Preferred = rect };

    public static T WithRect<T>(this T self, Rect r) where T : ComponentState
        => self with { Dimensions = self.Dimensions.WithRect(r) };

    public static T WithPreferredRect<T>(this T self, Rect r) where T : ComponentState
        => self with { Dimensions = self.Dimensions.WithPreferredRect(r) };

    public static IComponent With(this IComponent self, ComponentState state)
        => self.With(state, self.Children);

    public static IComponent With(this IComponent self, Func<ComponentState, ComponentState> func)
        => self.With(func(self.State));

    public static IComponent WithRect(this IComponent self, Rect r)
        => self.With(state => state.WithRect(r));

    public static Rect PreferredRect(this IComponent self)
        => self.State.Dimensions.Preferred;

    public static Rect ActualRect(this IComponent self)
        => self.State.Dimensions.Actual;

    // NOTE: it is concievable that self derives from a type that is also in the type hierarchy of state,
    // but this is too esoteric to deal with at the current time.
    public static T Merge<T>(this T self, ComponentState state) where T : ComponentState
        => state is T typedState ?
        typedState
        : self with
        {
            Active = state.Active,
            Dimensions = state.Dimensions,
            Enabled = state.Enabled,
            Hovered = state.Hovered,
            Visible = state.Visible,
            Rendered = state.Rendered,
        };
}
