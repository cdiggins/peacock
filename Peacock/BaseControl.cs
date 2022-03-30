namespace Peacock;

public record Control<TView>(TView View) : IControl 
    where TView : IView
{
    IView IControl.View => View;
    public virtual ICanvas Draw(ICanvas canvas) => canvas;
    public virtual IEnumerable<IControl> GetChildren(IControlFactory factory) => Enumerable.Empty<IControl>();
    public virtual IView Process(IInputEvent input, IDispatcher dispatcher) => View;
}