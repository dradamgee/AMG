using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AMG.FySics;
using Microsoft.FSharp.Collections;

namespace GraphicsSandbox
{
    public class LeashViewModel : ForceViewModel
    {
        private readonly Leash _leash;
        private readonly ElementViewModel _e1;

        public LeashViewModel(Leash leash, ElementViewModel e1)
        {
            _leash = leash;
            _e1 = e1;
            X2 = leash.Pin.X;
            Y2 = -leash.Pin.Y;

            e1.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Location")
                {
                    X1 = e1.Location.X;
                    Y1 = -e1.Location.Y;
                }
                OnPropertyChanged("Top");
                OnPropertyChanged("Left");
            };
        }

        public override FSharpList<AMG.FySics.PendingImpulse>  Act(double interval)
        {
            return _leash.Act(interval, _e1.Element);
        }
    }

    public class BondViewModel : ForceViewModel
    {
        private readonly Bond _bond;
        private readonly ElementViewModel _e1;
        private readonly ElementViewModel _e2;

        public BondViewModel(Bond bond, ElementViewModel e1, ElementViewModel e2)
        {
            _bond = bond;
            _e1 = e1;
            _e2 = e2;
            e1.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Location")
                {
                    X1 = e1.Location.X;
                    Y1 = -e1.Location.Y;
                }
                OnPropertyChanged("Bottom");
                OnPropertyChanged("Left");
            };
            e2.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Location")
                {
                    X2 = e2.Location.X;
                    Y2 = -e2.Location.Y;
                    OnPropertyChanged("Bottom");
                    OnPropertyChanged("Left");
                }
            };
        }

        public override FSharpList<PendingImpulse> Act(double interval)
        {
            return _bond.Act(interval, _e1.Element, _e2.Element);
        }
    }

    public abstract class ForceViewModel : INotifyPropertyChanged
    {
        public double Bottom
        {
            get { return 0; }
        }
        public double Left
        {
            get { return 0; }
        }

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
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract FSharpList<PendingImpulse> Act(double interval);
    }
}