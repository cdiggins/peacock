using System.Collections.Generic;
using System.Linq;

namespace Ned
{
    public class TestData
    {
        public static IReadOnlyList<Node> TestNodes
            => CreateNodes(Text).ToList();

        // Some things derive from other things 

        public static string Text =
@"Point
* Value *
* X *
* Y *
  Values[] *
--
Pair 
* Value *
* A *
* B *  
  Middle *
--
Line
* Value *
* A ▽ * 
* B ▽ *
  Middle *
  Length *   
--
Vector
* Value *
* X *
* Y * 
  Normal ▽ *
  Magnitude *
  Values[] *
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
* Position ▽ *
* Size ▽ *
  Center *
--
Arithmetic
* A *
* B *
  Add *
  Subtract *
  Multiply *
  Divide *
  Average *
  Interval * 
  Magnitude * 
--
Trig Ops
* Theta * 
  Sine *
  Cosine * 
  Tangent *
  Secant *
  Cosecant *
  Cotangent *
--
Common Ops
* Input *
  Negate *
  Inverse *
  Sign *
  Magnitude *
  Sqrt *  
--
Repeat
* Value *
* Count *
  Output[] *
--
Sequence
* Start *
* Count * 
  Output[] *
--
Pair
* A *
* B *
  Output[] *
--
Subsequence
* A[] *
* Interval *
  Output[] *
--
Element At
* A[] *
* Index *
  Output *
--
Where
* A[] *
* Filter[] *
  Output[] *
--
Conditional
* A *
* B *
* Condition *
  Output * 
--
Lerp
* Interval *
* Amount *
  Output *
--
Inverse Lerp
* Interval *
* Lerp *
  Output *
--
Interval
* Value *
* A *
* B *
* Low *
* High *
  Magnitude *
  Middle * 
--
Clamp 
* Input *
* Interval *
  Output *
--
Circle
* Value *
* Center *
* Radius * 
--
Chord
* Value *
* Circle *
* Interval *
--
Comparison
* A * 
* B *
  > *
  >= *
  < *
  <= *
  == *
  != *
  Max *
  Min *
--
Boolean
* A *
* B *
  And *
  Or *
  Nand *
  Nor *
  Xor *
  Not A *
  Not B *
--
Set 
* Set[] * 
* Element *
  Contains *
  Add[] *
  Remove[] * 
--
Set Ops
* SetA[] *
* SetB[] *
  Union *
  Difference *
  Intersection *
--
Sample
* Interval *
* Count * 
  Output[] *
  Random[] *
  Poissonian[] *
--
Array Ops
* Input[] *
  Reverse[] *
  Sort[] * 
  Shuffle[] *
  Count *
  First *
  Last *
  Range * 
-- 
Select Index
* Input[] * 
* Indices[] *
  Output[] *
";
        public static IEnumerable<Node> CreateNodes(string s)
            => s.Split("--").Where(x => !string.IsNullOrWhiteSpace(x)).Select(CreateNode);        

        public static Slot CreateSlot(string s)
            => new() 
            {
                Label = s.Trim().TrimStart('*', ' ').TrimEnd('*', ' ').Trim(),
                Left = s.Trim().StartsWith('*') ? new Socket() { LeftOrRight = true } : null,
                Right = s.Trim().EndsWith('*') ? new Socket() { LeftOrRight = false } : null
            };

        public static Node CreateNode(string s)
            => CreateNode(s.Trim().Split('\n'));

        public static Node CreateNode(IEnumerable<string> contents)
        {
            var label = contents.First();
            contents = contents.Skip(1);
            var first = contents.First();
            var slot = CreateSlot(first);

            var header = new Header() { Label = label };
            if (slot.Label == "Value")
            {
                header = header with { Left = slot.Left, Right = slot.Right };
                contents = contents.Skip(1);
            }

            var slots = contents.Select(CreateSlot).ToList();

            return new()
               {
                   Header = header,
                   Label = label,                      
                   Slots = slots,
               };
        }
    }
}
