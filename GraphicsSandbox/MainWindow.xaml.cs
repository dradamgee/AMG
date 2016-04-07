using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace GraphicsSandbox {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        readonly IDisposable _universe;

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
    }
}
