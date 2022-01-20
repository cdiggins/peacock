using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Peacock
{
    // TODO: this is stateful, so make it a "Unique" type when converting to Plato.

    public record WpfRenderer : ICanvas         
    {
        public DrawingContext Context { get; init; }

        public WpfRenderer(WindowProps props, DrawingContext context, Rect rect)
        {
            WindowProps = props;
            Context = context;
            Rect = rect;
            Context.PushTransform(new TranslateTransform(rect.Left, rect.Top));
            Context.PushClip(new RectangleGeometry(new Rect(rect.Size)));
        }

        public void Dispose()
        {
            Context.Pop();
            Context.Pop();
        }

        public class CanvasBrush : IBrush
        {
            public Brush Brush { get; init; }
        }

        public IBrush CreateBrush(Color color) 
            => new CanvasBrush { Brush = new SolidColorBrush(color) { Opacity = (double)color.A / 255 }};

        public class CanvasFont : IFont
        {
            public FontFamily FontFamily { get; init; }
            public double Size { get; init; }
        }

        public IFont CreateFont(string familyName, double size) 
            => new CanvasFont { FontFamily = new FontFamily(familyName),Size = size };

        public class CanvasPen : IPen
        {
            public Pen Pen { get; init; }
        }

        public IPen CreatePen(Color color, double width) 
            => new CanvasPen { Pen = new Pen(new SolidColorBrush(color), width) };

        public ICanvas DrawLine(IPen pen, Point p0, Point p1)
        {
            Context.DrawLine(((CanvasPen)pen).Pen, p0, p1);
            return this;
        }

        // var dpiInfo = VisualTreeHelper.GetDpi(visual);
        // From <https://stackoverflow.com/questions/58343299/formattedtext-and-pixelsperdip-if-application-is-scaled-independently-of-dpi> 
        // TODO: handle DPI properly
        // TODO: support different font styles, weights, and stretches

        public ICanvas DrawText(IFont font, IBrush brush, Point point, string text)
        {
            var localFont = (CanvasFont)font;
            var typeFace = new Typeface(localFont.FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeFace, localFont.Size, ((CanvasBrush)brush).Brush, new NumberSubstitution(), 1.0);
            Context.DrawText(formattedText, point);
            return this;
        }

        public ICanvas DrawEllipse(IBrush? brush, IPen? pen, Point center, double radiusX, double radiusY)
        {
            Context.DrawEllipse((brush as CanvasBrush)?.Brush, (pen as CanvasPen)?.Pen, center, radiusX, radiusY);
            return this;
        }

        public ICanvas DrawRect(IBrush? brush, IPen? pen, Rect rect)
        {
            Context.DrawRectangle((brush as CanvasBrush)?.Brush, (pen as CanvasPen)?.Pen, rect);
            return this;
        }

        public ICanvas DrawRoundedRect(IBrush? brush, IPen? pen, Rect rect, double radiusX, double radiusY)
        {
            Context.DrawRoundedRectangle(
                (brush as CanvasBrush)?.Brush, 
                (pen as CanvasPen)?.Pen, rect, radiusX, radiusY);
            return this;
        }

        public Rect Rect { get; init; }

        public ICanvas SetRect(Rect rect)
            => new WpfRenderer(WindowProps, Context, Rect);        

        public WindowProps WindowProps { get; init; }

        public ICanvas SetWindowProps(WindowProps props)
           => this with { WindowProps = props };
    }
}