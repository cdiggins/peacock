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

        public WpfRenderer(DrawingContext context)
        {
            Context = context;
        }

        public ICanvas SetRect(Rect rect)
        { 
            Context.PushTransform(new TranslateTransform(rect.Left, rect.Top));
            Context.PushClip(new RectangleGeometry(new Rect(rect.Size)));
            return this;
        }

        public ICanvas PopRect()
        {
            Context.Pop();
            Context.Pop();
            return this;
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

        public Typeface GetTypeface(IFont font)
            => new (((CanvasFont)font).FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

        public FormattedText GetFormattedText(IFont font, IBrush brush, string text)
            => new (text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, GetTypeface(font), ((CanvasFont)font).Size, ((CanvasBrush) brush).Brush, new NumberSubstitution(), 1.0); 

        // var dpiInfo = VisualTreeHelper.GetDpi(visual);
        // From <https://stackoverflow.com/questions/58343299/formattedtext-and-pixelsperdip-if-application-is-scaled-independently-of-dpi> 
        // TODO: handle DPI properly
        // TODO: support different font styles, weights, and stretches

        public ICanvas DrawText(IFont font, IBrush brush, Point point, string text)
        {
            var formattedText = GetFormattedText(font, brush, text);
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

        public Size MeasureString(IFont font, IBrush brush, string text)
        {
            var t = GetFormattedText(font, brush, text);
            return new Size(t.Width, t.Height);
        }
    }
}