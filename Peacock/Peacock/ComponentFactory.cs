using System.Windows;

namespace Peacock;

// A component factory allows themes, and such things. 
public class ComponentFactory
{
    public Style DefaultStyle => new();
    public StyledState DefaultStyledState => new(DefaultStyle);

    public LabelComponent Label(string text)
        => new (new LabelState(DefaultStyledState, text).WithPreferredRect(new(0, 0, 100, 25)));

    public ButtonComponent Button(string text)
        => new (new(DefaultStyledState), Label(text));

    public StackComponent Stack(Rect Rect, params IComponent[] children)
        => new (new StackState(DefaultStyledState).WithRect(Rect), children);

    public StackComponent Stack(params IComponent[] children)
        => new (new StackState(DefaultStyledState), children);

    public IComponent TitleBar(string text)
        => Label(text).With(state => state.WithPreferredRect(new(0,0,100,35)));

    public StackComponent Window(double width, double height, string text, IComponent content) 
        => Stack(new(0,0, width, height), new[] { TitleBar(text), content });
}
