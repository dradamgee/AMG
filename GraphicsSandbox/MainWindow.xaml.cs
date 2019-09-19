using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using AMG.FySics;
using Vector = AMG.FySics.Vector;

namespace GraphicsSandbox {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        readonly Universe _universe;

        public MainWindow() {
            InitializeComponent();

            //_universe = God.CreateUniverse();



            _universe = God.CreateUniverseFromFile(@"C:\_asd\log.txt");

            DataContext = _universe;
            
            Closing += OnClosing;
            Closed += (sender, args) => Thread.Sleep(1000);
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs){
            _universe.Dispose();
        }

        private void ItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _universe.Size = new Vector(e.NewSize.Width, e.NewSize.Height);
        }

        //private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    var position = e.GetPosition(this);
        //    var positionVector = new Vector(position.X, position.Y);
        //    var ball = new Ball(5, 20, positionVector, new Velocity(new Vector(100, 100)));
        //    _universe.Add(ball);
        //}
    }
}
