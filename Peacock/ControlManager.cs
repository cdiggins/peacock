using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Peacock
{
    public class Tree<T>
    {
        public T Value { get; }
        public IReadOnlyList<Tree<T>> Children { get; }

        public Tree(T x, Func<T, IEnumerable<Tree<T>>> childrenGenerator)
            => (Value, Children) = (x, childrenGenerator(x).ToList());

        public IEnumerable<Tree<T>> AllNodes()
        {
            yield return this;
            foreach (var c in Children)
                foreach (var n in c.AllNodes())
                    yield return n;
        }
    }

    /// <summary>
    /// The ControlManager class contains the current control tree, which it creates from a control factory.
    /// It also maintains the list of associated behaviors with each control. 
    /// The control factory can be updated at any time. 
    /// </summary>
    [Mutable]
    public class ControlManager
    {
        public ControlManager(IControlFactory factory)
            => Factory = factory;

        public void UpdateControlTree(IModel model, IControlFactory? newFactory = null)
        {
            Factory = newFactory ?? Factory;
            ControlTree = Factory.Create(model).Select(ToControlTree).ToList();
            FlushUnusedBehaviors();
        }

        public Dictionary<IControl, List<IBehavior>> Behaviors { get; } = new();
        public IControlFactory Factory { get; set; }
        public List<Tree<IControl>> ControlTree { get; set; } = new();
        public IEnumerable<IControl> AllControls => ControlTree.SelectMany(node => node.AllNodes()).Select(n => n.Value);

        public Tree<IControl> ToControlTree(IControl control)
            => new(control, x => x.GetChildren(Factory).Select(ToControlTree).ToList());

        /// <summary>
        /// Called during control tree updates, to assure that behaviors associated with controls don't stick around. 
        /// </summary>
        private void FlushUnusedBehaviors()
        {
            var controlHashSet = AllControls.ToHashSet();
            foreach (var control in Behaviors.Keys)
                if (!controlHashSet.Contains(control))
                    Behaviors.Remove(control);
        }

        public ICanvas Draw(ICanvas canvas) 
            => AllControls.Aggregate(canvas, (current, control) => control.Draw(current));

        public IUpdates ProcessInput(InputEvent input, IUpdates updates = null) 
            => AllControls.Aggregate(updates ?? new Updates(), (current, control) => control.Process(input, current));

        public static IBehavior ApplyUpdates(IBehavior behavior, IUpdates updates)
            => updates.BehaviorUpdates.ContainsKey(behavior)
                ? ApplyUpdates(behavior, updates.BehaviorUpdates[behavior])
                : behavior;

        public static IControl ApplyUpdates(IControl control, IUpdates updates)
            => updates.ControlUpdates.ContainsKey(control)
                ? ApplyUpdates(control, updates.ControlUpdates[control])
                : control;

        public void ApplyChanges(IUpdates updates)
        {
            // Add new behaviors 
            foreach (var (control, newBehaviors) in updates.NewBehaviors)
            {
                if (!Behaviors.ContainsKey(control))
                    Behaviors[control] = new();
                Behaviors[control].AddRange(newBehaviors);
            }

            foreach (var control in Behaviors.Keys.ToList())
            {
                // Update the behavior list
                Behaviors[control] = Behaviors[control]
                    .Select(b => ApplyUpdates(b, updates)).ToList();

                // Update the control if required 
                if (updates.ControlUpdates.ContainsKey(control))
                {
                    var updates = updater.ControlUpdates[control];
                    var newControl = updates.Aggregate(control, (local, func) => func(local));
                    Behaviors[newControl] = Behaviors[control];
                    Behaviors.Remove(control);
                }
            }

            // TODO: look for nulls (meaning the behavior or control is removed)
            // TODO: look for new controls. 
            // TODO: add new behaviors 
            // TODO: behaviors have to be hashed with the control, otherwise everyone will drag at once. 

            
            // Now: how do I update the model? 
            // Well each control is associated with a model. This class knows, because it created. 
            // So if the control changes its view, then the application should monitor for changes. 
            // How? It could iterate over the updates? 
            // It could store a callback somewhere? Like during construction? 
            // The idea is that there is one function .... in a control that is called. 
            // What does it do ... just update the model? I guess that could work. 
            // I could use behaviors ... but there seems to be no way to easily add behaviors? 
            // Well controls have "Default behaviors", which perhaps can be set upon construction of the control.
            // Ideally there is a way for the system to update just the view, and the application monitors those 
            // changes. The "CallBack" could work. 
            // I could stick such a callback in a control. It could also be returned as part of the "creation". 
            // Note: I can use mutable class. 
            // I would have thought that something like a "updates" could be passed to the class. 

            // So this is interesting: only one control. Only one "update". 
            // Updates are very nice ... just simple callback. The behavior could even be implemented in terms of this. 
            
            /*
             * Think about in terms of how to declare it first.
             *
             * 1) The control factory says: here is the control I want you to make.
             * 2) while creating it, it tells the control, when you are updated, here is how to tell me.
             *
             * So what does it get? A "Action(IControl control)"? That gets invoked? Simple enough I guess.
             *
             * Or alternatively, it does it through a behavior. So when the control is created it gets a list of default
             * behaviors, and one of them is triggered automatically whenever the control is changed?
             *
             * Well technically it isn't a behavior. 
             */
        }
    }
}
