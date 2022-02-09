using System;
using System.Windows;
using System.Windows.Media;

namespace Peacock;

// A component factory allows themes, and such things. 
public class ComponentFactory
{
    public static Style DefaultStyle => new();
    public static StyledState DefaultStyledState => new(DefaultStyle);

    public IComponent TextBlock(string text, Style blockStyle, Style textStyle)
        => new BackgroundComponent<StyledState>(new(blockStyle), new[] { Label(text, textStyle) });

    public IComponent Label(string text, Style? style = null)
        => new LabelComponent(new LabelState(new StyledState(style ?? DefaultStyle), text)).WithRect(new(0, 0, 100, 25));

    public IComponent Button(string text)
        => new ButtonComponent(new(DefaultStyledState), new[] { Label(text) });

    public IComponent Stack(Rect rect, params IComponent[] children)
        => new StackComponent(new StackState(DefaultStyledState), children).WithRect(rect);

    public IComponent Stack(params IComponent[] children)
        => new StackComponent(new StackState(DefaultStyledState), children);

    public IComponent TitleBar(string text)
        => TextBlock(text, 
            DefaultStyle with { BackgroundColor = Colors.Blue }, 
            DefaultStyle with
            {
                BackgroundColor = Colors.Blue,
                FontColor = Colors.AntiqueWhite,
                FontSize = 16,
                Alignment = Alignment.CenterCenter,
            })
            .With(state => state.WithRect(new(0, 0, 100, 35)));

    public IComponent Grid(params IComponent[] components)
        => new GridComponent(DefaultStyledState, components);

    public IComponent Window(double width, double height, string text, IComponent content)
        => Stack(new(0, 0, width, height), new[] { TitleBar(text).AddDraggable(), content });
}
        
