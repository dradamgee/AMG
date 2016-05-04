using AMG.Physics;
using AMG.FySics;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace GraphicsSandbox
{
    public abstract class Element : INotifyPropertyChanged, IElement
    {
        public double Mass { get; set; }

        public Element(Dimensions location, Velocity velocity)
        {
            Velocity = velocity;
            _location = location;
            Mass = 1.0d;
        }
        
        private Dimensions _location;
        public Velocity Velocity {get;set;}

        public Dimensions Location {
            get { return _location; }
            set {
                _location = value;
                OnPropertyChanged();
                OnPropertyChanged("Top");
                OnPropertyChanged("Left");
            }
        }
        
        public double Top {
            get { return Location.Y; }
        }

        public double Left {
            get { return Location.X; }
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