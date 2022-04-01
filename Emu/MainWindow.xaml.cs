using System.IO;
using System.Windows;
using Peacock;

namespace Emu;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();             
    }

    private void MenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        var canvas = new SvgCanvas(500, 500);
        GraphUserControl.Render(canvas);
        var text = canvas.ToString();
        File.WriteAllText(Path.Combine(Path.GetTempPath(), "temp.svg"), text);
    }
}