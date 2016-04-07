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

        public Universe(Dispatcher dispatcher) {
            Dispatcher = dispatcher;
            elements = new ObservableCollection<IElement>();
            elements.Add(new Ball(4, new Dimensions(100, 100)));
            elements.Add(new Square(4, new Dimensions(150, 150) ));
            elements.Add(new Square(4, new Dimensions(150, 150)));
            elements.Add(new Square(4, new Dimensions(150, 150)));
            elements.Add(new Square(4, new Dimensions(150, 150)));
            elements.Add(new Square(4, new Dimensions(150, 150)));
            elements.Add(new Square(4, new Dimensions(150, 150)));
            elements.Add(new Square(4, new Dimensions(150, 150)));
            elements.Add(new Square(4, new Dimensions(150, 150)));
            elements.Add(new Square(4, new Dimensions(150, 150)));

            

            Gravity gravity = new Gravity(98, elements);
            _boundry = new Boundry(new Dimensions(525, 350), elements);
                                   
            timeDependentActions = new List<TimeDependentAction>();

            WireCollisions(elements.ToArray());
            timeDependentActions.Add(gravity);
            timeDependentActions.Add(_boundry);
            timeDependentActions.AddRange(elements.Select(e => e.Velocity));

            _cancellationTokenSource = new CancellationTokenSource();
            time = new UniversalTime(timeDependentActions, _cancellationTokenSource.Token);
        }

        public void WireCollisions(IElement[] elements) {
            for(int i = 0; i < elements.Count(); i++)
            {
                for (int j = i+1; j < elements.Count(); j++)
                {
                    timeDependentActions.Add(new Collision(4, elements[i], elements[j]));
                }
            }
        }

        public ObservableCollection<IElement> elements {
            get;
            set;
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

