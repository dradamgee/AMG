using System.Collections.Generic;
using AMG.Physics;
using AMG.FySics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace GraphicsSandbox
{
    public abstract class Element : INotifyPropertyChanged, IElement
    {
        private static int nextId = 0;
        private ICommand elementCommand;

        public double Mass { get; set; }

        public abstract IEnumerable<Element> Split();

        public ICommand ElementCommand
        {
            get { return elementCommand; }
            set
            {
                elementCommand = value;
                OnPropertyChanged();
            }
        }

        public Element(double mass, Vector location, Velocity velocity)
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