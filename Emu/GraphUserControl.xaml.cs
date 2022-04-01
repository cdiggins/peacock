using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Peacock;
using Peacock.Wpf;
using Dispatcher = Peacock.Dispatcher;

namespace Emu;

/// <summary>
/// Interaction logic for GraphUserControl.xaml
/// TODO: a lot of this could go into the Peacock.Wpf layer.
/// </summary>
public partial class GraphUserControl : UserControl
{
    public IObjectStore Store = new ObjectStore();
    public IControlFactory Factory = new ControlFactory();
    public IControl Control;
    public WpfRenderer Canvas = new();
    public int WheelZoom = 0;
    public double ZoomFactor => Math.Pow(1.15, WheelZoom / 120.0);
    public Graph Graph;
    public DrawingGroup CurrentFrame = new();
    public DrawingGroup NextFrame = new ();

    public Dictionary<IControl, DrawingGroup> Lookup = new();

    public DispatcherTimer Timer = new()
    {
        Interval = TimeSpan.FromMilliseconds(20)
    };

    public DateTimeOffset Started;

    public class MouseStatus : IMouseStatus
    {
        public MouseStatus(GraphUserControl control) => Control = control;
        public GraphUserControl Control { get; }
        public Point Location => Mouse.GetPosition(Control).Multiply(1.0 / Control.ZoomFactor);
        public bool LButtonDown => Mouse.LeftButton == MouseButtonState.Pressed;
        public bool RButtonDown => Mouse.RightButton == MouseButtonState.Pressed;
        public bool MButtonDown => Mouse.MiddleButton == MouseButtonState.Pressed;
    }

    public GraphUserControl()
    {
        InitializeComponent();
        Focusable = true;
        Focus();

        // TODO: I have to create a RefList of Nodes and Connections. 
        // This indicates I have an object store for the model. I suppose that makes sense. 
        // Like the ControlManager. 
        
        Graph = TestData.CreateGraph(Store);

        //(this.Parent as Window).PreviewKeyDown += (sender, args) => Console.WriteLine("Parent key press");
        PreviewKeyDown += (sender, args) => ProcessInput(new KeyDownEvent(args));
        PreviewKeyUp += (sender, args) => ProcessInput(new KeyUpEvent(args));
        PreviewMouseDoubleClick += (sender, args) => ProcessInput(new MouseDoubleClickEvent(args));
        PreviewMouseDown += (sender, args) => ProcessInput(new MouseDownEvent(args));
        PreviewMouseUp += (sender, args) => ProcessInput(new MouseUpEvent(args));
        PreviewMouseMove += (sender, args) => ProcessInput(new MouseMoveEvent(args));
        PreviewMouseWheel += (sender, args) => ProcessInput(new MouseWheelEvent(args));
        SizeChanged += (sender, args) => ProcessInput(new ResizeEvent(args));
            
        // Animation timer
        Timer.Tick += (sender, args) => ProcessInput(new ClockEvent((DateTimeOffset.Now - Started).TotalSeconds));
        Timer.Start();
        Started = DateTimeOffset.Now;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        base.OnPreviewKeyDown(e);
    }

    public static IEnumerable<IControl> GetControls(IControlFactory factory, IControl parent)
        => parent.GetChildren(factory).SelectMany(child => GetControls(factory, child)).Prepend(parent);

    public void Draw(ICanvas canvas, IEnumerable<IControl> controls)
    {
        //foreach (var control in controls) canvas = control.Draw(canvas);
        if (canvas is WpfRenderer dc)
        {
            foreach (var control in controls)
                dc.Context.DrawDrawing(Lookup[control]);
        }
    }

    public DrawingGroup ToDrawingGroup(IControl control)
    {
        var dg = new DrawingGroup();
        var context = dg.Open();
        control.Draw(new WpfRenderer(context));
        context.Close();
        return dg;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        //drawingContext.DrawDrawing(CurrentFrame);
        Render(drawingContext);
        base.OnRender(drawingContext);
    }

    public void Render(DrawingContext drawingContext)
    {
        var rect = new Rect(new(), RenderSize);
        drawingContext.PushClip(new RectangleGeometry(rect));
        drawingContext.DrawRectangle(new SolidColorBrush(Colors.SlateGray), new Pen(), rect);
        var scaleTransform = new ScaleTransform(ZoomFactor, ZoomFactor);
        drawingContext.PushTransform(scaleTransform);
        Canvas.Context = drawingContext;
        Render(Canvas);
        drawingContext.Pop();
        drawingContext.Pop();
    }

    public void Render(ICanvas canvas)
    {
        var rootControl = Factory.Create(null, Graph);
        List<IControl> controls = new List<IControl>();
        var creatingProfiler = Util.TimeIt("Create controls", () => controls = GetControls(Factory, rootControl).ToList());
        if (Lookup.Count == 0)
        {
            foreach (var c in controls)
            {
                Lookup.Add(c, ToDrawingGroup(c));
            }
        }
        else
        {
            foreach (var c in controls)
            {
                Debug.Assert(Lookup.ContainsKey(c));
            }
        }

        var drawingProfiler = Util.TimeIt("Drawing", () => Draw(Canvas, controls));
        var averageCreationTime = creatingProfiler.AverageMsec();
        var averageDrawingTime = drawingProfiler.AverageMsec();
    }

    public void ProcessInput<T>(T inputEvent)
        where T : InputEvent
    {
        if (inputEvent is MouseWheelEvent mwe)
        {
            WheelZoom += mwe.Args.Delta;
        }

        var updates = new Dispatcher();
        inputEvent.MouseStatus = new MouseStatus(this);

        /*
        (var newGraphControl, updates) = _graphControl.ProcessInput(updates, inputEvent);
        if (newGraphControl == _graphControl)
            return;
        _graphControl = newGraphControl;

        var newView = _graphControl.View.ApplyUpdates(updates);
        _graphControl = _graphControl.UpdateView(newView);
        */

        /*
        if (inputEvent is ClockEvent)
        {
            NextFrame = new();
            var context = NextFrame.Open();
            Render(context);
            context.Close();
            CurrentFrame = NextFrame;
        }
        */
        InvalidateVisual();
    }
}