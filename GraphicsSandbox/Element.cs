using System.ComponentModel;
using System.Runtime.CompilerServices;
using AMG.Physics;

namespace GraphicsSandbox
{
    public class Element : INotifyPropertyChanged, IElement
    {
        public Element(Dimensions location)
        {
            Velocity = new Velocity(this) {Dimensions = new Dimensions(20, 30)};
            _location = location;
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
    }
}