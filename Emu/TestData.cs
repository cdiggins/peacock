using Peacock;
using System.Collections.Generic;
using System.Linq;

namespace Emu;

public class TestData
{
    public static string Text =
            
        @"Point 2D
* Value *
* X : Number *
* Y : Number *
  Values[] *
--
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

    public static (IObjectStore, IEnumerable<Node>) CreateNodes(IObjectStore store, string s)
    {
        var nodes = new List<Node>();
        foreach (var subString in s.Split("--").Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            (store, var node) = CreateNode(store, subString);
            nodes.Add(node);
        }

        return (store, nodes);
    }

    public static (IObjectStore, Slot) CreateSlot(IObjectStore store, string s, string nodeName, bool isHeader)
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

        var leftSocket = hasLeftSocket ? new Socket(type, true) : null;
        var rightSocket = hasRightSocket ? new Socket(type, false) : null;
        store = leftSocket == null ? store : store.Add(leftSocket);
        store = rightSocket == null ? store : store.Add(rightSocket);
        var slot = new Slot(name, type, isHeader, leftSocket, rightSocket);
        return (store.Add(slot), slot);
    }

    public static (IObjectStore, Node) CreateNode(IObjectStore store, string s)
        => CreateNode(store, s.Trim().Split('\n', System.StringSplitOptions.RemoveEmptyEntries).ToList());

    public static (IObjectStore, Node) CreateNode(IObjectStore store, List<string> contents)
    {
        var label = contents[0].Trim();
        const NodeKind kind = NodeKind.OperatorSet;             
        var header = new Slot(label, label, true, null, null);
        var slots = new List<Slot>();
        foreach (var c in contents)
        {
            (store, var slot) = CreateSlot(store, c, label, false);
            slots.Add(slot);
        }

        var r = new Node(label, kind, header, slots.ToRefList());
        return (store.Add(r), r);
    }

    public static (IObjectStore, Graph) CreateGraph(IObjectStore? store = null)
    {
        store ??= new ObjectStore();
        (store, var nodes) = CreateNodes(store, Text);
        return (store, new Graph(nodes.ToRefList(), RefList<Connection>.Empty);
    }
}