using System.Windows;
using System.Windows.Input;

namespace Peacock
{
    // Keyboard, mouse, touch, file system, other 
    public record InputEvent(object Value);

    public record KeyDownEvent(KeyboardEventArgs Args) : InputEvent(Args);
    public record KeyUpEvent(KeyboardEventArgs Args) : InputEvent(Args);
    public record MouseDoubleClickEvent(MouseButtonEventArgs Args, Point Point) : InputEvent(Args);
    public record MouseDownEvent(MouseButtonEventArgs Args, Point Point) : InputEvent(Args);
    public record MouseUpEvent(MouseButtonEventArgs Args, Point Point) : InputEvent(Args);
    public record MouseMoveEvent(MouseEventArgs Args, Point Point) : InputEvent(Args);
    public record MouseWheelEvent(MouseWheelEventArgs Args, Point Point) : InputEvent(Args);
    public record ResizeEvent(SizeChangedEventArgs Args) : InputEvent(Args);
    public record ClockEvent(double ElapsedSeconds) : InputEvent(ElapsedSeconds);
}


