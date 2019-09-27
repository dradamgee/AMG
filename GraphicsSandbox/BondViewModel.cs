using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AMG.FySics;

namespace GraphicsSandbox
{
    public class BondViewModel : INotifyPropertyChanged
    {
        public Bond Bond { get; }

        public double Top
        {
            get { return 0; }
        }
        public double Left
        {
            get { return 0; }
        }

        private Element e1;
        private Element e2;

        private double _x1; private double _y1;
        private double _x2; private double _y2;

        public double X1
        {
            get { return _x1; }
            set
            {
                _x1 = value;
                OnPropertyChanged();
            }
        }

        public double Y1
        {
            get { return _y1; }
            set
            {
                _y1 = value;
                OnPropertyChanged();
            }
        }

        public double X2
        {
            get { return _x2; }
            set
            {
                _x2 = value;
                OnPropertyChanged();
            }
        }

        public double Y2
        {
            get { return _y2; }
            set
            {
                _y2 = value;
                OnPropertyChanged();
            }
        }

        public BondViewModel(Bond bond)
        {
            Bond = bond;
            e1 = bond.E1 as Element;
            e2 = bond.E2 as Element;

            e1.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Location")
                {
                    X1 = e1.Location.X;
                    Y1 = e1.Location.Y;
                }
                OnPropertyChanged("Top");
                OnPropertyChanged("Left");
            };
            e2.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Location")
                {
                    X2 = e2.Location.X;
                    Y2 = e2.Location.Y;
                    OnPropertyChanged("Top");
                    OnPropertyChanged("Left");
                }
            };
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}