using AMG.Physics;

namespace GraphicsSandbox
{
    public class Ball : Element{
        private int m_radius;

        public Ball(int diameter, Dimensions location) : base(location)
        {
            m_radius = diameter;
        }

        public int Diameter {
            get { return m_radius * 2; }
        }

        public override double Radius {
            get { return m_radius; }
        }
    }
}