using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacock
{
    public interface IControlTree
    {
        IReadOnlyList<IControlTree> Children { get; }

        IControl Control { get; }
    }

    public class ControlTree : IControlTree
    {
        public ControlManager
    }
