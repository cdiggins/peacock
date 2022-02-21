using System.Windows;
using Peacock;

namespace Bohr;

// TODO: there is a set of mapping the views to actual controls 
// It provides the styled details. For example given a SocketView, it can give the proper 

// TODO: the problem with the current approach is that some items might have control specific stuff ... they won't get carried forward.

public static class Dimensions
{
    public static double NodeWidth => 110;
    public static double NodeSpacing => 20;
    public static double NodeSlotHeight => 20;
    public static double NodeHeaderHeight => 25;
    public static Radius SocketRadius => new(6, 6);
    public static Radius SlotRadius => NodeRadius;
    public static Radius NodeRadius => new(8, 8);
    public static double NodeHeight(int slots) => NodeHeaderHeight + slots * NodeSlotHeight;
    public static double NodeBorder => 3;
    public static double ConnectionWidth => 5;
    public static double SocketBorder => 1;
    public static double HeaderTextSize => 14;
    public static double SlotTextSize => 12;
    public static double SlotTypeTextSize => 6;
    public static Size SlotTextOffset => new(SocketRadius.X * 1.5, 0);
}
