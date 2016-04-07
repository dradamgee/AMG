namespace GraphicsSandbox
{
    public class Square : Element{
        private int m_size;

        public int Size
        {
            get { return m_size; }
            set
            {
                m_size = value;
                OnPropertyChanged();
            }
        }

        public Square(int size, Dimensions location) : base(location)
        {
            Size = size;
        }
    }
}