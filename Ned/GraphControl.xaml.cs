using System;
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

        public DispatcherTimer Timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(13)
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
            MouseWheel += (sender, args) => ProcessInput(new MouseWheelEvent(args));
            SizeChanged += (sender, args) => ProcessInput(new ResizeEvent(args));

            // Animation timer
            Timer.Tick += (sender, args) => ProcessInput(new ClockEvent((DateTimeOffset.Now - Started).TotalSeconds));
            Timer.Start();
            Started = DateTimeOffset.Now;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            _graphControl.Draw(new WpfRenderer(drawingContext));
        }

        public void ProcessInput<T>(T inputEvent)
            where T : InputEvent
        {
            inputEvent.Element = this;
            _graphControl = _graphControl.ProcessInput(inputEvent);
            InvalidateVisual();
        }
    }
}
