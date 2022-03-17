namespace Peacock;

/// <summary>
/// A behavior is associated with a control and can provide additional drawing capabilities.
/// Multiple behaviors can be applied to a control. 
/// A behavior can process input and update itself in response to user input.
/// Some example use cases of behaviors:
/// * Drawing a border
/// * Animation effects on specific event MouseMove/Enter/Leve/Down
/// * Changing cursor in response to events 
/// * Tooltips when mouse is hovered
/// * Managing and representing enabled/disabled state. 
/// * Making a control draggable
/// </summary>
public interface IBehavior : IObject
{
    public ICanvas Draw(IControl control, ICanvas canvas);
    public (IUpdates, IBehavior) ProcessInput(IControl control, IUpdates updates, InputEvent input);
}

