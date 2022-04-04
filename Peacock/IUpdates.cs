namespace Peacock;

/// <summary>
/// Represents a collection of proposed changes to the UI or application state.
/// An implementation of this class is supplied by the Peacock library.
/// 
/// These changes are aggregated and applied to the UI state by the control manager,
/// and then to the model by the application.
/// 
/// The IUpdates serves a similar role as the Reducer in a React application that uses
/// the Redux state management library. 
///
/// The ControlManager class is responsible for applying changes from an IUpdates to the internal
/// controls and behaviors, and triggering any IControl.Callback calls as well.
/// When controls are changed, they will be given an opportunity to apply model changes
/// to the IUpdates.
///
/// An application is responsible for looking at the updates and iterating over
/// all proposed changes to the model and applying them. 
/// </summary>
public interface IUpdates
{
    IUpdates AddBehavior(IControl control, IBehavior behavior);
    IUpdates UpdateBehavior(IBehavior key, Func<IBehavior, IBehavior> updateFunc);
    IUpdates UpdateControl(IControl key, Func<IControl, IControl> updateFunc);
    IUpdates UpdateModel(IModel key, Func<IModel, IModel> updateFunc);

    IEnumerable<IControl> UpdatedControls();
    IEnumerable<IBehavior> UpdatedBehaviors();
    IEnumerable<IModel> UpdatedModels();
    IEnumerable<IBehavior> NewBehaviors(IControl control);

    IControl ApplyToControl(IControl control);
    IBehavior ApplyToBehavior(IBehavior behavior);
    IModel ApplyToModel(IModel model);
}

public static class UpdatesExtensions
{
    public static T ApplyToModel<T>(this IUpdates updates, T model) where T : IModel
        => (T)updates.ApplyToModel(model);

    public static T ApplyToControl<T>(this IUpdates updates, T control) where T : IControl
        => (T)updates.ApplyToControl(control);

    public static T ApplyToBehavior<T>(this IUpdates updates, T behavior) where T : IBehavior
        => (T)updates.ApplyToBehavior(behavior);
}