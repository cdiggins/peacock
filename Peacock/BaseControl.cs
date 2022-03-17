using System.Reflection;

namespace Peacock;

public abstract record BaseControl<TView> : Object, IControl
    where TView : IView
{
    protected BaseControl(TView view) => (View, Ctor) = (view, GetType().GetConstructor(new[] { typeof(TView) })!);
    public TView View { get; }
    IView IControl.View => View!;
    public virtual ICanvas Draw(ICanvas canvas) => canvas;
    IView IControl.Process(IInputEvent input) => Process(input)!;
    public IControl With(IView view) => With((TView)view);
    public virtual TView Process(IInputEvent input) => View;
    public ConstructorInfo Ctor { get; }
    public virtual IControl With(TView view) => (IControl)Ctor.Invoke(new object?[] { View });
}