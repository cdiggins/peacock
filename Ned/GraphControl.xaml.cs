using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ned
{
    /// <summary>
    /// Interaction logic for GraphControl.xaml
    /// </summary>
    public partial class GraphControl : UserControl
    {
        private Graph _graph;
        private GraphView _graphView;
        private IControl _graphControl;
        public WpfRenderer Renderer = new WpfRenderer();
        public int WheelZoom = 0;

        public DispatcherTimer Timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(25)
        };

        public DateTimeOffset Started;

        public Graph Graph
        {
            get => _graph;
            set
            {
                _graph = value;
                _graphView = _graph.ToView(_graphView);
                _graphControl = _graphView.ToControl();
                InvalidateVisual();
            }
        }        

        public GraphControl()
        {
            InitializeComponent();
            Graph = new Graph { Label = "My Graph", Nodes = TestData.TestNodes };

            KeyDown += (sender, args) => ProcessInput(new KeyDownEvent(args));
            KeyUp += (sender, args) => ProcessInput(new KeyUpEvent(args));
            MouseDoubleClick += (sender, args) => ProcessInput(new MouseDoubleClickEvent(args));
            MouseDown += (sender, args) => ProcessInput(new MouseDownEvent(args));
            MouseUp += (sender, args) => ProcessInput(new MouseUpEvent(args));
            MouseMove += (sender, args) => ProcessInput(new MouseMoveEvent(args));
            PreviewMouseWheel += (sender, args) => ProcessInput(new MouseWheelEvent(args));
            SizeChanged += (sender, args) => ProcessInput(new ResizeEvent(args));

            // Animation timer
            Timer.Tick += (sender, args) => ProcessInput(new ClockEvent((DateTimeOffset.Now - Started).TotalSeconds));
            Timer.Start();
            Started = DateTimeOffset.Now;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var rect = new Rect(new(), RenderSize);
            drawingContext.PushClip(new RectangleGeometry(rect));
            drawingContext.DrawRectangle(new SolidColorBrush(Colors.SlateGray), new Pen(), rect);
            double zoomFactor = Math.Pow(1.15, WheelZoom / 120.0);
            var scaleTransform = new ScaleTransform(zoomFactor, zoomFactor);
            drawingContext.PushTransform(scaleTransform);
            Renderer.Context = drawingContext;            
            _graphControl.Draw(Renderer);
            drawingContext.Pop();
            drawingContext.Pop();
            base.OnRender(drawingContext);
        }

        public void ProcessInput<T>(T inputEvent)
            where T : InputEvent
        {
            if (inputEvent is MouseWheelEvent mwe)
            {
                WheelZoom += mwe.Args.Delta;
            }

            inputEvent.Element = this;
            var newGraphControl = _graphControl.ProcessInput(inputEvent);
            if (newGraphControl == _graphControl)
                return;
            _graphControl = newGraphControl;
            InvalidateVisual();
        }
    }
}
