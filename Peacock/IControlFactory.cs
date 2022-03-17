namespace Peacock;

/// <summary>
/// This provides a mapping from models to controls.
/// Using a factory simplifies theming and making broad changes.
/// Factories are implemented by applications. 
/// </summary>
public interface IControlFactory
{
    IControl CreateControl(IControl parent, IObject model);
    ControlManager CreateChildren(ControlManager manager, IControl parent);
}