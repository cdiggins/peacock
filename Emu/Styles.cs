using System.Windows;
using System.Windows.Media;
using Peacock;

namespace Emu;

public record ShapeStyle(Color Color, Color BorderColor, double BorderWidth, Radius CornerRadius);
public record TextStyle(Color Color, string FontFamily, double FontSize, Alignment Alignment);
public record SlotStyle(ShapeStyle ShapeStyle, TextStyle TextStyle, TextStyle SmallTextStyle, Rect Dimensions);
public record SocketStyle(ShapeStyle ShapeStyle, TextStyle TextStyle, Rect Dimensions);
public record NodeStyle(ShapeStyle ShapeStyle, TextStyle TextStyle, Rect Dimensions);
public record ConnectionStyle(ShapeStyle ShapeStyle, TextStyle TextStyle);
public record GraphStyle(ShapeStyle ShapeStyle, TextStyle TextStyle);