using System.Collections.Generic;
using AMG.Physics;
using AMG.FySics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace GraphicsSandbox
{
    public abstract class ElementViewModel : INotifyPropertyChanged, IElement
    {
        private static int nextId = 0;
        private ICommand _expandCommand;
        private ICommand _collapseCommand;

        public double Mass { get; set; }

        public abstract IEnumerable<ElementViewModel> Split();

        public ICommand ExpandCommand
        {
            get { return _expandCommand; }
            set
            {
                _expandCommand = value;
                OnPropertyChanged();
            }
        }

        public ICommand CollapseCommand
        {
            get { return _collapseCommand; }
            set
            {
                _collapseCommand = value;
                OnPropertyChanged();
            }
        }

        public ElementViewModel(double mass, Vector location, Velocity velocity)
        {
            Id = nextId++;
            Velocity = velocity;
            _location = location;
            Mass = mass;
        }
        
        private Vector _location;
        public Velocity Velocity {get;set;}

        public int Id { get; private set; }

        public Vector Location {
            get { return _location; }
            set {
                _location = value;
                OnPropertyChanged();
                OnPropertyChanged("Top");
                OnPropertyChanged("Left");
                OnPropertyChanged("Bottom");
                OnPropertyChanged("Right");
            }
        }
        
        public double Top {
            get { return Location.Y - Radius; }
        }

        public double Left {
            get { return Location.X - Radius; }
        }

        public double Bottom
        {
            get { return Location.Y + Radius; }
        }

        public double Right
        {
            get { return Location.X + Radius; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract double Radius { get; }

        public override string ToString()
        {
            return Mass + "|" + Radius + "|" + Location + "|" + Velocity;
        }
    }
}