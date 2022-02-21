using System;
using System.Collections.Generic;
using System.Linq;

namespace Bohr
{
    public class TestData
    {
        public static IReadOnlyList<Node> TestNodes
            => CreateNodes(Text).ToList();

        // Some things derive from other things 

        public static string Text =
            
@"Point 2D
* Value *
* X : Number *
* Y : Number *
  Values[] *";

            public static string MoreText = 
                @"--
Mouse
  Position : Point 2D *
  Left : Boolean *
  Middle : Boolean *
  Right : Boolean * 
--
Keyboard
  Keys : Array * 
--
Time
  Seconds : Number *  
--
Pair 
* Value *
* A : Any *
* B : Any *  
--
Line
* Value *
* A : Point 2D * 
* B : Point 2D *
  Middle : Point 2D *
  Length : Number *   
--
Vector 2D
* Value *
* X : Number *
* Y : Number * 
  Normal : Vector 2D *
  Magnitude : Number *
  Values : Array *
--
Size 2D
* Value * 
* Width : Number * 
* Height : Number *
  Magnitude : Number *
--
Rect
* Value *
* Position : Point 2D *
* Size : Size 2D *
  Center : Point 2D *
--
Arithmetic
* A : Any *
* B : Any *
  Add : Any *
  Subtract : Any *
  Multiply : Any *
  Divide : Any *
  Average : Any *
  Interval : Any * 
  Magnitude : Any * 
--
Trig Ops
* Theta : Angle * 
  Sine : Number *
  Cosine : Number * 
  Tangent : Number *
  Secant : Number *
  Cosecant : Number *
  Cotangent : Number *
--
Inv Trig Ops
* Input : Number * 
  ArcSine : Angle *
  ArcCosine : Angle * 
  ArcTangent : Angle *
  ArcSecant : Angle *
  ArcCosecant : Angle *
  ArcCotangent : Angle *
--
Number Ops
* Input : Number *
  Negate : Number *
  Inverse : Number *
  Sign : Number *
  Magnitude : Number *
  Sqrt : Number *  
  Deg To Rad : Number *
  Rad to Deg : Number *
--
Repeat
* Element : Any *
* Count : Integer *
  Output : Array *
--
Sequence
* Start : Integer *
* Count : Integer * 
  Output : Array *
--
Pair
* A : Any *
* B : Any *
  Output : Array *
--
Subsequence
* A : Array *
* Interval : Interval *
  Output : Array *
--
Element At
* A : Array *
* Index : Integer *
  Output : Array *
--
Where
* A : Array *
* Filter : Array *
  Output : Array *
--
Conditional
* A : Any *
* B : Any *
* Condition : Boolean *
  Output : Any * 
--
Lerp
* Interval : Interval *
* Amount : Number *
  Output : Any *
--
Inverse Lerp
* Interval : Interval *
* Lerp : Any *
  Output : Number *
--
Interval
* Value *
* A : Any *
* B : Any *
* Low : Any *
* High : Any *
  Magnitude : Number *
  Middle : Any * 
--
Clamp 
* Input : Any *
* Interval : Interval *
  Output : Any *
--
Circle
* Value *
* Center : Point 2D *
* Radius : Number * 
--
Chord
* Value *
* Circle : Circle *
* Interval : Interval *
--
Comparison
* A : Any * 
* B : Any *
  > : Boolean *
  >= : Boolean *
  < : Boolean *
  <= : Boolean *
  == : Boolean *
  != : Boolean *
  Max : Any *
  Min : Any *
--
Boolean
* A : Boolean *
* B : Boolean *
  And : Boolean *
  Or : Boolean *
  Nand : Boolean *
  Nor : Boolean *
  Xor : Boolean *
  Not A : Boolean *
  Not B : Boolean *
--
Set 
* Set : Array * 
* Element : Any *
  Contains : Boolean*
  Add : Array *
  Remove : Array * 
--
Set Ops
* A : Array *
* B : Array *
  Union : Array *
  Difference : Array *
  Intersection : Array *
--
Sample
* Interval : Interval *
* Count : Integer * 
  Output : Array *
  Random : Array *
  Poissonian : Array *
--
Array Ops
* Input : Array *
  Reverse : Array *
  Sort : Array * 
  Shuffle : Array *
  Count : Integer *
  First : Any *
  Last : Any *
  Range : Any * 
-- 
Select Index
* Input : Array * 
* Indices : Array *
  Output : Array *
--
Bezier Curve
* Start : Any *
* Control A : Any *
* Control B : Any *
* End : Any *
* Amount : Number * 
  Output : Any * 
--
Transform 2D
* Value *
* Translation : Vector 2D *
* Scale : Size 2D * 
* Rotation : Angle *
";
        public static IEnumerable<Node> CreateNodes(string s)
            => s.Split("--").Where(x => !string.IsNullOrWhiteSpace(x)).Select(CreateNode);

        public static Slot CreateSlot(string s, string nodeName)
        {
            s = s.Trim();

            var hasLeftSocket = s.StartsWith("*");
            var hasRightSocket = s.EndsWith("*");

            s = s.TrimStart('*', ' ').TrimEnd('*', ' ').Trim();

            var n = s.IndexOf(':');
            var name = s.Trim();
            var type = nodeName.Trim();

            if (n >= 0)
            {
                name = s.Substring(0, n - 1).Trim();
                type = s.Substring(n + 1).Trim();
            }

            return new Slot(name, type, hasLeftSocket ? new Socket(type, true) : null, hasRightSocket ? new Socket(type, false) : null);
        }

        public static Node CreateNode(string s)
            => CreateNode(s.Trim().Split('\n', System.StringSplitOptions.RemoveEmptyEntries));

        public static Node CreateNode(IEnumerable<string> contents)
        {
            var label = contents.First().Trim();
            contents = contents.Skip(1);
            var first = contents.First().Trim();
            var slot = CreateSlot(first, label);
            var kind = NodeKind.OperatorSet;             
            var header = new Header(label, null, null);

            if (slot.Label == "Value")
            {
                header = new Header(slot.Type, slot.Left, slot.Right);
                contents = contents.Skip(1);
                kind = NodeKind.PropertySet;
            }

            var r = new Node(label, kind, header, contents.Select(c => CreateSlot(c, label)).ToList());

            if (!r.Slots.Any(s => s.Left != null))
            {
                r = r with { Kind = NodeKind.Output };
            }
            if (!r.Slots.Any(s => s.Right != null))
            {
                r = r with { Kind = NodeKind.Input };
            }

            return r;
        }
    }
}
