using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacock
{
    [Mutable]
    public class UIState
    {
        public DictionaryOfLists<IControl, IBehavior> Behaviors { get; } = new();
        public IControlFactory Factory { get; set; }

        public void ApplyChanges(Dispatcher dispatcher)
        {
            foreach (var control in Behaviors.GetKeys())
            {
                // 
            }
        }

        public void UpdateControls(IModel model)
        {
            // Recomputes the model
        }

        public void ProcessInput()
        {
        }

        public void Draw(ICanvas canvas)
        {}
    }
}
