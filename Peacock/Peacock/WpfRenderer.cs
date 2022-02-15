using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Ned
{
    // TODO: this is stateful, so make it a "Unique" type when converting to Plato.

    public record WpfRenderer : ICanvas         
    {
        public DrawingContext Context { get; init; }

        public WpfRenderer(DrawingContext context)
            => Context = context;


        public ICanvas WithContext(Action<DrawingContext> action)
        {
            action(Context);
            return this;
        }
 
        public ICanvas Draw(StyledText text)
            => WithContext(context => context.DrawText(
                CreateFormattedText(text), 
                context.Pop

        public ICanvas Draw(StyledLine line)
            => WithContext(context => context.DrawLine(
                CreatePen(line.PenStyle), 
                line.Line.A, 
                line.Line.B));

        public ICanvas Draw(StyledEllipse ellipse)
            => WithContext(context => context.DrawEllipse(
                CreateBrush(ellipse.Style.BrushStyle),
                CreatePen(ellipse.Style.PenStyle),
                ellipse.Ellipse.Point,
                ellipse.Ellipse.Radius.X,
                ellipse.Ellipse.Radius.Y));

        public ICanvas Draw(StyledRect rect)
            => WithContext(context => context.DrawRoundedRectangle(
                CreateBrush(rect.Style.BrushStyle),
                CreatePen(rect.Style.PenStyle),
                rect.Rect.Rect,
                rect.Rect.Radius.X,
                rect.Rect.Radius.Y));

        public Size MeasureText(StyledText text)
        {
            var t = CreateFormattedText(text);
            return new Size(t.Width, t.Height);
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

        public static Brush CreateBrush(BrushStyle style)
            => CreateBrush(style.Color);

        public static Brush CreateBrush(Color color) 
            => new SolidColorBrush(color) { Opacity = (double)color.A / 255 };

        public static Pen CreatePen(PenStyle style)
            => new(CreateBrush(style.BrushStyle), style.Width);

        public static Typeface CreateTypeface(TextStyle style)
            => new(style.FontFamily);

        public static FormattedText CreateFormattedText(StyledText text)
            => new (text.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, CreateTypeface(text.Style), text.Style.FontSize, 
                CreateBrush(text.Style.BrushStyle), new NumberSubstitution(), 1.0); 

        // var dpiInfo = VisualTreeHelper.GetDpi(visual);
        // From <https://stackoverflow.com/questions/58343299/formattedtext-and-pixelsperdip-if-application-is-scaled-independently-of-dpi> 
        // TODO: handle DPI properly
        // TODO: support different font styles, weights, and stretches
    }
}