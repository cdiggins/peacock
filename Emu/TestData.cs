using System;
using Peacock;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Emu;

[Mutable]
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

    public static IReadOnlyList<Node> CreateNodes(IObjectStore store, string s)
    {
        var nodes = new List<Node>();
        var pos = new Point(20, 20);
        foreach (var subString in s.Split("--").Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            // TODO: compute the Rect. 
            var node = CreateNode(store, pos, subString);
            pos = pos.Add(new Size(NodeWidth * 1.3, 0));
            nodes.Add(node);
        }

        return nodes;
    }

    public static Guid NewGuid() => Guid.NewGuid();

    public static int NodeHeaderHeight = 25;
    public static int NodeSlotHeight = 20;
    public static int NodeWidth = 110;
    public static int SlotRadius = 5;

    public static double GetNodeHeight(int slots) => NodeHeaderHeight + slots * NodeSlotHeight;
    public static Rect GetNodeHeaderRect(Rect nodeRect) => new(nodeRect.TopLeft, new Size(nodeRect.Width, NodeHeaderHeight));
    public static Rect GetSocketRect(Rect slotRect, bool leftOrRight) => GetSocketRect(leftOrRight ? slotRect.LeftCenter() : slotRect.RightCenter());
    public static Rect GetSocketRect(Point point) => new(point.X - SlotRadius, point.Y - SlotRadius, SlotRadius * 2, SlotRadius * 2);
    public static Rect GetSlotRect(Rect rect, int i) => new(rect.Left, rect.Top + NodeHeaderHeight + i * NodeSlotHeight, rect.Width, NodeSlotHeight);
    public static Slot CreateSlot(IObjectStore store, Rect nodeRect, int index, string s, string nodeName, bool isHeader)
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

        var slotRect = GetSlotRect(nodeRect, index);
        var leftSocket = hasLeftSocket ? new Socket(NewGuid(), GetSocketRect(slotRect, true), type, true) : null;
        var rightSocket = hasRightSocket ? new Socket(NewGuid(), GetSocketRect(slotRect, false), type, false) : null;
        store = leftSocket == null ? store : store.Add(leftSocket);
        store = rightSocket == null ? store : store.Add(rightSocket);
        var slot = new Slot(NewGuid(), slotRect, name, type, isHeader, leftSocket, rightSocket);
        store.Add(slot);
        return slot;
    }

    public static Node CreateNode(IObjectStore store, Point pos, string s)
        => CreateNode(store, pos, s.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList());

    public static Node CreateNode(IObjectStore store, Point pos, List<string> contents)
    {
        var label = contents[0].Trim();
        const NodeKind kind = NodeKind.OperatorSet;

        var rect = new Rect(pos, new Size(NodeWidth, GetNodeHeight(contents.Count)));
        var header = new Slot(new Guid(), GetNodeHeaderRect(rect), label, label, true, null, null);
        var slots = new List<Slot>();
        for (var i=0; i < contents.Count; ++i)
        {
            var c = contents[i];
            var slot = CreateSlot(store, rect, i, c, label, false);
            slots.Add(slot);
        }

        var r = new Node(NewGuid(), rect, label, kind, header, slots);
        store.Add(r);
        return r;
    }

    public static Graph CreateGraph(IObjectStore store)
    {
        var nodes = CreateNodes(store, Text);
        var r = new Graph(NewGuid(), nodes, Array.Empty<Connection>());
        store.Add(r);
        return r;

    }
}