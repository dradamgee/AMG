using AMG.FySics;
using AMG.Physics;

namespace GraphicsSandbox
{
    public class Square : Element{
        private int _sideLength;
        private int _radius;

        public int SideLength
        {
            get { return _sideLength; }
            set
            {
                _sideLength = value;
                _radius = value / 2;
                OnPropertyChanged();
            }
        }

        public Square(double mass, int sideLength, Vector location, Velocity velocity) : base(mass, location, velocity)
        {
            SideLength = sideLength;
        }

        public override double Radius
        {
            get { return _radius; }
        }
    }
}