using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ned
{
    public class TestData
    {
        public static IReadOnlyList<Node> TestNodes
            => ToNodes(Text).ToList();

        public static string Text = 
@"Point
* Value *
* X *
* Y *
--
Vector
* Value *
* X *
* Y * 
Length *
Size *
--
Size
* Value *
* Width * 
* Height *
Vector *
Magnitude *
--
Rect
* Value *
* Position *
* Size *
Center *
";
        public static IEnumerable<Node> ToNodes(string s)
            => s.Split("--").Where(x => !string.IsNullOrWhiteSpace(x)).Select(Node);        

        public static Slot Slot(string s)
            => new() 
            {
                Label = s.TrimStart('*', ' ').TrimEnd('*', ' ').Trim(),
                Left = s.StartsWith('*') ? new Socket() { LeftOrRight = true } : null,
                Right = s.EndsWith('*') ? new Socket() { LeftOrRight = false } : null
            };

        public static Node Node(string s)
            => Node(s.Trim().Split('\n'));

        public static Node Node(IEnumerable<string> contents)
            => new()
            {
                Label = contents.First(),
                Slots = contents.Skip(1).Select(Slot).ToList()
            };
    }
}
