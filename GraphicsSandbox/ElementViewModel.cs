using System.Collections.Generic;
using AMG.Physics;
using AMG.FySics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace GraphicsSandbox
{
    public abstract class ElementViewModel : INotifyPropertyChanged
    {
        public Element Element
        {
            get => _element;
            set
            {
                _element = value;
                OnPropertyChanged("Location");
                OnPropertyChanged("Top");
                OnPropertyChanged("Left");
                OnPropertyChanged("Bottom");
                OnPropertyChanged("Right");
            }
        }

        private static int nextId = 0;

        public ElementViewModel(double mass, Vector location, Velocity velocity, double radius)
        {
            Element = new Element(nextId++, location, velocity, mass, radius);
        }
        
        public int Id=> Element.Id;
        public double Mass => Element.Mass;
        public Vector Location => Element.Location;
        public double Radius => Element.Radius;

        private ICommand _expandCommand;
        private ICommand _collapseCommand;

        public double Top => ElementModule.top(Element);
        public double Left => ElementModule.left(Element);
        public double Bottom => ElementModule.bottom(Element);
        public double Right => ElementModule.right(Element);


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

        private Vector _location;
        private Element _element;

        public Velocity Velocity
        {
            get { return Element.Velocity; }
        }

       
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return Mass + "|" + Radius + "|" + Location + "|" + Velocity;
        }
    }
}