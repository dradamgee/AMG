using AMG.Physics;
using AMG.FySics;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace GraphicsSandbox
{
    public abstract class Element : INotifyPropertyChanged, IElement
    {
        public double Mass { get; set; }

        public Element(double mass, Vector location, Velocity velocity)
        {
            Velocity = velocity;
            _location = location;
            Mass = mass;
        }
        
        private Vector _location;
        public Velocity Velocity {get;set;}

        public Vector Location {
            get { return _location; }
            set {
                _location = value;
                OnPropertyChanged();
                OnPropertyChanged("Top");
                OnPropertyChanged("Left");
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