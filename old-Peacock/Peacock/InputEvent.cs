using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Peacock;

public interface IComponentEvent
{ }

// This identifies type that contain input event data
// This could be generated from the Window system or from parent component 
public interface IInputEvent
{ }

// Used for collecting events 
public interface IEventAggregator
{
    IEventAggregator AddEvent(IComponentBase component, IComponentEvent input);
    IReadOnlyList<(IComponentBase, IComponentEvent)> Events { get; }
}

public record EventAggregator(IReadOnlyList<(IComponentBase, IComponentEvent)> Events) 
    : IEventAggregator
{
    public IEventAggregator AddEvent(IComponentBase component, IComponentEvent input)
        => new EventAggregator(Events.Append((component, input)).ToList());

    public static IEventAggregator Empty
        = new EventAggregator(Array.Empty<(IComponentBase, IComponentEvent)>());
}

// Keyboard, mouse, touch, file system, other 
public record InputEvent : IInputEvent;

public record KeyDownEvent(KeyboardEventArgs Args) : InputEvent;
public record KeyUpEvent(KeyboardEventArgs Args) : InputEvent;
public record MouseDoubleClickEvent(MouseButtonEventArgs Args, Point Point) : InputEvent;
public record MouseDownEvent(MouseButtonEventArgs Args, Point Point) : InputEvent;
public record MouseUpEvent(MouseButtonEventArgs Args, Point Point) : InputEvent;
public record MouseMoveEvent(MouseEventArgs Args, Point Point) : InputEvent;
public record MouseWheelEvent(MouseWheelEventArgs Args, Point Point) : InputEvent;
public record ResizeEvent(SizeChangedEventArgs Args) : InputEvent;
public record ClockEvent(double ElapsedSeconds) : InputEvent;
public record PropsEvent(WindowProps Props) : InputEvent;

public record DragStartEvent(Point Point) : IComponentEvent;
public record DragMoveEvent(Point Delta) : IComponentEvent;
public record DragEndEvent() : IComponentEvent;