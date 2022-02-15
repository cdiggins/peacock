﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Ned;

public static class Extensions
{
    public static Rect Shrink(this Rect r, Rect padding)
        => r.Shrink(padding.Left, padding.Top, padding.Right, padding.Bottom);

    public static Rect Shrink(this Rect r, double left, double top, double right, double bottom)
        => new Rect(r.Left + left, r.Top + top, r.Width - left - right, r.Height - top - bottom);

    public static Rect Shrink(this Rect r, double padding)
        => r.Shrink(padding, padding, padding, padding);

    public static Rect GetRow(this Rect r, int row, int rowCount)
        => new(r.Left, r.Top + r.Height * row / rowCount, r.Width, r.Height / rowCount);

    public static Rect GetColumn(this Rect r, int col, int colCount)
        => new(r.Left + r.Width * col / colCount, r.Top, r.Width / colCount, r.Bottom);

    public static Rect GetRowColumn(this Rect r, int row, int col, int rowCount, int colCount)
        => r.GetRow(row, rowCount).GetColumn(col, colCount);

    public static IEnumerable<Rect> GetGrid(this Rect r, int rowCount, int colCount)
        => Enumerable.Range(0, rowCount * colCount).Select(i => r.GetRowColumn(i / colCount, i % colCount, rowCount, colCount));

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

    public static Rect Resize(this Rect rect, Size size)
        => new(rect.Location, size);

    public static Rect MoveTo(this Rect rect, Point point)
        => new(point, rect.Size);

    public static Point Add(this Point self, Point point)
        => new(self.X + point.X, self.Y + point.Y);

    public static Rect MoveBy(this Rect rect, Point point)
        => new(rect.Location.Add(point), rect.Size);

    public static Point Negate(this Point point)
        => new(-point.X, -point.Y);

    public static Point Subtract(this Point self, Point point)
        => self.Add(point.Negate());

    public static Point GetScreenPosition(this MouseEventArgs args, Window window)
        => window.PointToScreen(args.GetPosition(window));

    public static double HalfHeight(this Rect rect)
        => rect.Size.HalfHeight();

    public static double HalfHeight(this Size size)
        => size.Height / 2.0;

    public static double HalfWidth(this Rect rect)
        => rect.Size.HalfWidth();

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

    public static Point GetAlignedLocation(this Rect rect, Size size, Alignment alignment)
        =>
        new(
            alignment.X switch
            {
                AlignmentX.Right => rect.Right - size.Width,
                AlignmentX.Center => rect.Center().X - size.HalfWidth(),
                _ => rect.Left
            },
            alignment.Y switch
            {
                AlignmentY.Bottom => rect.Bottom - size.Height,
                AlignmentY.Center => rect.Center().Y - size.HalfHeight(),
                _ => rect.Top
            });

    public static bool NonZero(this Rect self)
        => self.Width > 0 && self.Height > 0;

    public static Size GetSize(this FormattedText Text)
        => new(Text.Width, Text.Height);

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> self) where T : class
        => self.Where(x => x != null);

    public static IEnumerable<T> WhereNotNull<T>(params T?[] self) where T : class
        => self.WhereNotNull();
    
    public static Size Subtract(this Size size, Size amount)
        => new(size.Width - amount.Width, size.Height - amount.Height);

    public static Rect Shrink(this Rect rect, Size size)
        => new(rect.Location, rect.Size.Subtract(size));

    public static Rect Offset(this Rect rect, Size size)
        => new(rect.Location.Add(size), rect.Size);

    public static Point Add(this Point point, Size size)
        => new(point.X + size.Width, point.Y + size.Height);

    public static Rect ShrinkAndOffset(this Rect rect, Size size)
        => new(rect.Location.Add(size), rect.Size.Subtract(size));

    public static IReadOnlyList<T> Add<T>(this IReadOnlyList<T> self, T item)
        => self.Append(item).ToList();

    public static IReadOnlyList<T> Remove<T>(this IReadOnlyList<T> self, T item)
        => self.Where(x => x != null && !ReferenceEquals(x, item) && !x.Equals(item)).ToList();
}