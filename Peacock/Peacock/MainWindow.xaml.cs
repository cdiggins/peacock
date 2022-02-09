using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Peacock;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public DrawingGroup BackingStore = new DrawingGroup();
    public DispatcherTimer Timer = new DispatcherTimer
    {
        Interval = TimeSpan.FromMilliseconds(13)
    };
    public DateTimeOffset Started;

    public static ComponentFactory Factory = new ComponentFactory();
    public IComponent Widget = Factory.Window(800, 640, "My Window", Factory.Grid(
        Factory.Button("Press me"),
        Factory.Button("Me too"),
        Factory.Button("Not me"))
        );
    public new static readonly Style Style = new Style();

    public MainWindow()
    {
        InitializeComponent();

        KeyDown += (sender, args) => ProcessInput(new KeyDownEvent(args));
        KeyUp += (sender, args) => ProcessInput(new KeyUpEvent(args));
        MouseDoubleClick += (sender, args) => ProcessInput(new MouseDoubleClickEvent(args, args.GetScreenPosition(this)));
        MouseDown += (sender, args) => ProcessInput(new MouseDownEvent(args, args.GetScreenPosition(this)));
        MouseUp += (sender, args) => ProcessInput(new MouseUpEvent(args, args.GetScreenPosition(this)));
        MouseMove += (sender, args) => ProcessInput(new MouseMoveEvent(args, args.GetScreenPosition(this)));
        MouseWheel += (sender, args) => ProcessInput(new MouseWheelEvent(args, args.GetScreenPosition(this)));
        SizeChanged += (sender, args) => ProcessInput(new ResizeEvent(args));

        // Animation timer
        Timer.Tick += Timer_Tick;
        Timer.Start();
        Started = DateTimeOffset.Now;

        Debug.WriteLine(Widget.ToDebugString());
    }

    private void Timer_Tick(object? sender, EventArgs e)
        => ProcessInput(new ClockEvent((DateTimeOffset.Now - Started).TotalSeconds));

    public Rect RenderRect
        => new(RenderSize);

    public void ProcessInput<T>(T inputEvent)
        where T : InputEvent
    {
        var aggregator = EventAggregator.Empty;
        (var newWidget, aggregator) = Widget.ProcessInput(inputEvent, aggregator);
        if (newWidget == null || newWidget.Equals(inputEvent))
        {
            return;
        }
        Widget = newWidget;
        if (aggregator.GetEvents<DragStartEvent>().Any())
        {
            Cursor = Cursors.Hand;
        }
        if (aggregator.GetEvents<DragEndEvent>().Any())
        {
            Cursor = Cursors.Arrow;
        }
        var e = aggregator.GetEvents<DragMoveEvent>().FirstOrDefault();
        if (e != null)
        {
            Cursor = Cursors.Hand;
            (Left, Top) = (e.Delta.X, e.Delta.Y);
        }
        Render();
        InvalidateVisual();
    }

    public ICanvas Draw(DrawingContext dc)
        => Widget.Draw(new WpfRenderer(dc).SetRect(RenderRect)).PopRect();

    public void Render()
    {
        var drawingContext = BackingStore.Open();
        var canvas = Draw(drawingContext);
        drawingContext.Close();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        Render();
        drawingContext.DrawDrawing(BackingStore);
    }
}
