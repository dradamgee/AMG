using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;
using AMG.Physics;

namespace GraphicsSandbox {
    public class Universe : INotifyPropertyChanged, IDisposable
    {
        public static Dispatcher Dispatcher;
        
        UniversalTime time;
        CancellationTokenSource _cancellationTokenSource;
        private Boundry _boundry;
        List<TimeDependentAction> timeDependentActions;
        private int height;

        public Universe(Dispatcher dispatcher) {
            Dispatcher = dispatcher;
            Elements = new ObservableCollection<IElement>();

            int i = 200;
            while (i-- > 1)
            {
                Elements.Add(NewSquare());
                Elements.Add(NewBall());
            }

            Gravity gravity = new Gravity(300, Elements);
            _boundry = new Boundry(new Dimensions(525, 350), Elements);
                                   
            timeDependentActions = new List<TimeDependentAction>();

            timeDependentActions.Add(new StatefullCollisionDetector(Elements));
            //timeDependentActions.Add(new PairCollisionDetector(Elements));

            timeDependentActions.Add(gravity);
            timeDependentActions.Add(_boundry);
            timeDependentActions.AddRange(Elements.Select(e => e.Velocity));

            _cancellationTokenSource = new CancellationTokenSource();
            time = new UniversalTime(timeDependentActions, _cancellationTokenSource.Token);
        }



        private Square NewSquare()
        {
            return new Square(4, new Dimensions(RandomX, RandomY));
        }

        private Ball NewBall() {
            return new Ball( 5, new Dimensions(RandomX, RandomY));
        }

        Random random = new Random();

        private int RandomX {
            get { return random.Next(525); }
        }
        private int RandomY {
            get { return random.Next(350); }
        }





        public ObservableCollection<IElement> Elements {
            get;
            set;
        }

        public Dimensions Size
        {
            get
            {
                return _boundry.Dimensions;
            }
            set
            {
                _boundry.Dimensions = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose() 
        {
            _cancellationTokenSource.Cancel();
        }
    }
}

