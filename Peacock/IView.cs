namespace Peacock;

/// <summary>
/// A view is the strict set of data required to provide an interactive visual representation.
/// In other words it is the state of a control. 
/// </summary>
public interface IView
{
    Guid Id { get; }
    IView Apply(Func<IView, IView> func);
}