using AMG.Physics;

namespace GraphicsSandbox
{
    public class Ball : Element{
        private int m_diameter;

        public Ball(int diameter, Dimensions location) : base(location)
        {
            Diameter = diameter;
        }

        public int Diameter {
            get { return m_diameter; }
            set
            {
                m_diameter = value;
                OnPropertyChanged();
            }
        }
    }
}