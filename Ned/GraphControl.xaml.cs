using Peacock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ned
{
    /// <summary>
    /// Interaction logic for GraphControl.xaml
    /// </summary>
    public partial class GraphControl : UserControl
    {
        private Graph _graph;
        private GraphView _graphView;
        private IComponent _graphComponent;

        public Graph Graph
        {
            get => _graph;
            set
            {
                _graph = value;
                _graphView = _graph.ToView(_graphView);
                _graphComponent = CreateComponent(_graphView);
                InvalidateVisual();
            }
        }        

        public GraphControl()
        {
            InitializeComponent();
            Graph = new Graph { Label = "My Graph", Nodes = TestData.TestNodes };
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // TODO: 
            _graphComponent.Draw(new WpfRenderer(drawingContext));
        }

        public static IComponent CreateComponent(GraphView graphView)
        {
            // TODO:
            throw new NotImplementedException();
        }
    }
}
