using AMG.FySics;
using AMG.Physics;

namespace GraphicsSandbox
{
    public class Ball : Element{
        private int m_radius;

        public Ball(int diameter, Dimensions location, Velocity velocity) : base(location, velocity)
        {
            m_radius = diameter;
        }

        public int Diameter {
            get { return m_radius * 2; }
        }

        public override double Radius {
            get { return m_radius; }
        }

        public static implicit operator Ball(string s)
        {
            var xx = s.Split('|');
            return new Ball(
                int.Parse(xx[1]), 
                new Dimensions(double.Parse(xx[2]), double.Parse(xx[3])), 
                new Velocity(
                    new Dimensions(double.Parse(xx[4]), double.Parse(xx[5])
                    )));
        }
    }
}