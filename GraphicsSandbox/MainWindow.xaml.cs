using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using AMG.Physics;

namespace GraphicsSandbox {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        readonly Universe _universe;

        public MainWindow() {
            InitializeComponent();

            _universe = new Universe(Dispatcher);
            DataContext = _universe;
            
            Closing += OnClosing;
            Closed += (sender, args) => Thread.Sleep(1000);
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs){
            _universe.Dispose();
        }

        private void ItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _universe.Size = new Dimensions(e.NewSize.Width, e.NewSize.Height);
        }
    }
}
