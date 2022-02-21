using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Peacock;

namespace Bohr
{
    /// <summary>
    /// Interaction logic for GraphControl.xaml
    /// </summary>
    public partial class GraphControl : UserControl
    {
        private IControl _graphControl;
        public WpfRenderer Renderer = new WpfRenderer();
        public int WheelZoom = 0;
        public double ZoomFactor => Math.Pow(1.15, WheelZoom / 120.0);

        public DispatcherTimer Timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(25)
        };

        public DateTimeOffset Started;

        public class MouseStatus : IMouseStatus
        {
            public MouseStatus(GraphControl control) => Control = control;
            public GraphControl Control { get; }
            public Point Location => Mouse.GetPosition(Control).Multiply(1.0 / Control.ZoomFactor);
            public bool LButtonDown => Mouse.LeftButton == MouseButtonState.Pressed;
            public bool RButtonDown => Mouse.RightButton == MouseButtonState.Pressed;
            public bool MButtonDown => Mouse.MiddleButton == MouseButtonState.Pressed;
        }

        public GraphControl()
        {
            InitializeComponent();
            Focusable = true;
            Focus();
            var graph = new Graph(TestData.TestNodes, new Connection[] { });
            _graphControl = new GraphView(graph).ToControl();
            

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

        protected override void OnRender(DrawingContext drawingContext)
        {
            var rect = new Rect(new(), RenderSize);
            drawingContext.PushClip(new RectangleGeometry(rect));
            drawingContext.DrawRectangle(new SolidColorBrush(Colors.SlateGray), new Pen(), rect);
            var scaleTransform = new ScaleTransform(ZoomFactor, ZoomFactor);
            drawingContext.PushTransform(scaleTransform);
            Renderer.Context = drawingContext;            
            _graphControl.Draw(Renderer);
            drawingContext.Pop();
            drawingContext.Pop();
            base.OnRender(drawingContext);
        }

        public class Updates : IUpdates
        {
            public Dictionary<Guid, List<Func<IView, IView>>> _lookup { get; } = new();
            public IReadOnlyDictionary<Guid, List<Func<IView, IView>>> Lookup => _lookup;

            public IUpdates AddUpdate(Guid id, Func<IView, IView> func)
            {
                if (!Lookup.ContainsKey(id))
                {
                    _lookup.Add(id, new());
                }
                _lookup[id].Add(func);
                return this;
            }
        }

        public void ProcessInput<T>(T inputEvent)
            where T : InputEvent
        {
            if (inputEvent is MouseWheelEvent mwe)
            {
                WheelZoom += mwe.Args.Delta;
            }

            var updates = new Updates() as IUpdates;
            inputEvent.MouseStatus = new MouseStatus(this);
            (var newGraphControl, updates) = _graphControl.ProcessInput(updates, inputEvent);
            if (newGraphControl == _graphControl)
                return;
            _graphControl = newGraphControl;

            var newView = _graphControl.View.ApplyUpdates(updates);
            _graphControl = _graphControl.UpdateView(newView);
            
            InvalidateVisual();
        }
    }
}
