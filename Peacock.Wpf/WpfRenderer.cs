﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Peacock.Wpf;

[Mutable]
public record WpfRenderer : ICanvas         
{
    public DrawingContext? Context { get; set; }

    public Dictionary<BrushStyle, Brush> Brushes = new();
    public Dictionary<PenStyle, Pen> Pens = new();
    public Dictionary<TextStyle, Typeface> Typefaces = new();
    public Dictionary<StyledText, FormattedText> FormattedTexts = new();

    // TODO: figure out how to integrate this.
    public Dictionary<IControl, DrawingGroup> DrawingGroup = new();

    public static TValue GetOrCreate<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> func)
        where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out var value))
            return value;
        value = func(key);
        dictionary.Add(key, value);
        return value;
    }

    public ICanvas WithContext(Action<DrawingContext> action)
    {
        if (Context != null)
            action(Context);
        return this;
    }

    public ICanvas Draw(StyledText text) 
        => WithContext(context => context.DrawText( GetFormattedText(text), GetTextLocation(text)));

    public Point GetTextLocation(StyledText text)
        => text.Rect.GetAlignedLocation(MeasureText(text), text.Style.Alignment);

    public ICanvas Draw(StyledLine line)
        => WithContext(context => context.DrawLine(
            GetPen(line.PenStyle), 
            line.Line.A, 
            line.Line.B));

    public ICanvas Draw(StyledEllipse ellipse)
        => WithContext(context => context.DrawEllipse(
            GetBrush(ellipse.Style.BrushStyle),
            GetPen(ellipse.Style.PenStyle),
            ellipse.Ellipse.Point,
            ellipse.Ellipse.Radius.X,
            ellipse.Ellipse.Radius.Y));

    public ICanvas Draw(StyledRect rect)
        => WithContext(context => context.DrawRoundedRectangle(
            GetBrush(rect.Style.BrushStyle),
            GetPen(rect.Style.PenStyle),
            rect.Rect.Rect,
            rect.Rect.Radius.X,
            rect.Rect.Radius.Y));

    public Size MeasureText(StyledText text)
        => GetFormattedText(text).GetSize();

    public ICanvas SetRect(Rect rect)
    { 
        Context?.PushTransform(new TranslateTransform(rect.Left, rect.Top));
        Context?.PushClip(new RectangleGeometry(new Rect(rect.Size)));
        return this;
    }

    public ICanvas PopRect()
        => WithContext(context => { context.Pop(); context.Pop(); });

    public Brush GetBrush(BrushStyle style)
        => GetOrCreate(Brushes, style, style => new SolidColorBrush(style.Color) { Opacity = (double)style.Color.A / 255 });

    public Pen GetPen(PenStyle style)
        => GetOrCreate(Pens, style, style => new(GetBrush(style.BrushStyle), style.Width));

    public Typeface GetTypeface(TextStyle style)
        => GetOrCreate(Typefaces, style, style => new(style.FontFamily));

    public FormattedText ToFormattedText(StyledText text)
        => new(text.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, GetTypeface(text.Style),
            text.Style.FontSize, GetBrush(text.Style.BrushStyle), new NumberSubstitution(), 1.0);

    public FormattedText GetFormattedText(StyledText style)
        => GetOrCreate(FormattedTexts, style, x => ToFormattedText(x));
    
    // var dpiInfo = VisualTreeHelper.GetDpi(visual);
    // From <https://stackoverflow.com/questions/58343299/formattedtext-and-pixelsperdip-if-application-is-scaled-independently-of-dpi> 
    // TODO: handle DPI properly
    // TODO: support different font styles, weights, and stretches

    public ICanvas Draw(BrushStyle brushStyle, PenStyle penStyle, Geometry geometry)
        => WithContext(context => context.DrawGeometry(GetBrush(brushStyle), GetPen(penStyle), geometry));

    // TODO: 
    public DrawingGroup ToDrawingGroup(IControl control)
    {
        var dg = new DrawingGroup();
        var context = dg.Open();
        control.Draw(new WpfRenderer { Context = context });
        context.Close();
        return dg;
    }
}