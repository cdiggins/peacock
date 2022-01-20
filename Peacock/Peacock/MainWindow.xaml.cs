using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Peacock
{
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

        public static ComponentFactory Factory;
        public IWidget Widget = Theme.CreateWindow(800, 640, "My Window", Theme.CreateButton("Press me"));
        public new static readonly Style Style = new Style();       
        public WindowProps Props { get; set; }
            
        public MainWindow()
        {
            InitializeComponent();
            KeyDown += (sender, args) => ProcessInput(new KeyDownEvent(args));
            KeyUp += (sender, args) => ProcessInput(new KeyUpEvent(args));
            MouseDoubleClick += (sender, args) => ProcessInput(new MouseDoubleClickEvent(args, args.GetPosition(this)));
            MouseDown += (sender, args) => ProcessInput(new MouseDownEvent(args, args.GetPosition(this)));
            MouseUp += (sender, args) => ProcessInput(new MouseUpEvent(args, args.GetPosition(this)));
            MouseMove += (sender, args) => ProcessInput(new MouseMoveEvent(args, args.GetPosition(this)));
            MouseWheel += (sender, args) => ProcessInput(new MouseWheelEvent(args, args.GetPosition(this)));
            SizeChanged += (sender, args) => ProcessInput(new ResizeEvent(args));

            // Animation timer
            Timer.Tick += Timer_Tick;
            Timer.Start();
            Started = DateTimeOffset.Now;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            var newWidget = Widget.Update(this.GetProps(), new ClockEvent((DateTimeOffset.Now - Started).TotalSeconds));
            if (!newWidget.Equals(Widget))
            {
                Widget = newWidget;
                Render();
            }
        }

        // TODO: would be nice to make this async 
        // https://stackoverflow.com/questions/23442543/using-async-await-with-dispatcher-begininvoke

        public Rect RenderRect
            => new(RenderSize);

        public void ProcessInput<T>(T inputEvent)
            where T : InputEvent
        {
            Widget = Widget.Update(this.GetProps(), inputEvent);
            Render();
            InvalidateVisual();
        }

        public ICanvas Draw(DrawingContext dc)
            => Widget.Draw(new WpfRenderer(this.GetProps(), dc, RenderRect));

        public void Render()
        {
            var drawingContext = BackingStore.Open();
            var canvas = Draw(drawingContext);
            drawingContext.Close();

            // TODO: check that equals does return true, if the items are equivalent. 
            if (!Props?.Equals(canvas.WindowProps) ?? false)
            {
                Cursor = Props.Cursor;
                this.ApplyProps(Props);
                Props = canvas.WindowProps;
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            Render();
            drawingContext.DrawDrawing(BackingStore);
        }
    }
}
