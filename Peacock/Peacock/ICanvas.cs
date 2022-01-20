using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Peacock
{
    public record WindowProps(Rect Rect, string Title, Cursor Cursor);

    public interface IBrush
    {
    }

    public interface IPen
    {
    }

    public interface IFont
    {
    }

    public interface ICanvas : IDisposable
    {
        IBrush CreateBrush(Color color);
        IPen CreatePen(Color color, double width = 1.0);
        IFont CreateFont(string fontFamily, double size);

        ICanvas DrawLine(IPen pen, Point p0, Point p1);
        ICanvas DrawText(IFont font, IBrush brush, Point point, string text);
        ICanvas DrawEllipse(IBrush? brush, IPen? pen, Point center, double radiusX, double radiusY);
        ICanvas DrawRect(IBrush? brush, IPen? pen, Rect rect);
        ICanvas DrawRoundedRect(IBrush? brush, IPen? pen, Rect rect, double radiusX, double radiusY);

        WindowProps WindowProps { get; }
        ICanvas SetWindowProps(WindowProps props);

        ICanvas SetRect(Rect rect);
    }
}