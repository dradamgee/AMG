﻿using System;
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
            Elements.Add(new Ball(8, new Dimensions(100, 100)));
            Elements.Add(new Square(8, new Dimensions(154, 154)));
            Elements.Add(new Square(8, new Dimensions(158, 158)));
            Elements.Add(new Square(8, new Dimensions(160, 151)));
            Elements.Add(new Square(8, new Dimensions(170, 152)));
            Elements.Add(new Square(8, new Dimensions(180, 153)));
            Elements.Add(new Square(8, new Dimensions(190, 154)));
            Elements.Add(new Square(8, new Dimensions(200, 155)));
            Elements.Add(new Square(8, new Dimensions(220, 156)));
            Elements.Add(new Square(8, new Dimensions(2100, 157)));

            

            Gravity gravity = new Gravity(98, Elements);
            _boundry = new Boundry(new Dimensions(525, 350), Elements);
                                   
            timeDependentActions = new List<TimeDependentAction>();

            WireCollisions(Elements.ToArray());
            timeDependentActions.Add(gravity);
            timeDependentActions.Add(_boundry);
            timeDependentActions.AddRange(Elements.Select(e => e.Velocity));

            _cancellationTokenSource = new CancellationTokenSource();
            time = new UniversalTime(timeDependentActions, _cancellationTokenSource.Token);
        }

        public void WireCollisions(IElement[] elements) {
            for (int i = 0; i < elements.Count(); i++) {
                for (int j = i + 1; j < elements.Count(); j++) {
                    timeDependentActions.Add(new Collision(8, elements[i], elements[j]));
                }
            }
        }

        public Dimensions Size {
            get { return _boundry.Dimensions; }
            set { _boundry.Dimensions = value; }
        }

        public ObservableCollection<IElement> Elements {
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

