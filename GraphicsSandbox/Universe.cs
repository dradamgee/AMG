using AMG.FySics;
using AMG.Physics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;

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


            var e1 = (Ball)"1 | 8 | 196.5716629592 | 225.237471658389 | 4.96841730694 | 5.77965926300911";
            var e2 = (Ball)"1 | 8 | 185.307610553024 | 214.62117119859 | 15.1709404889631 | -5.65428572085577";

              


            Elements.Add(e1);
            Elements.Add(e2);


            int i = 0;
            while (i-- > 0)
            {
                //Elements.Add(NewSquare());
                Elements.Add(NewBall());

            }
            
            Gravity gravity = new Gravity(100);
            var collisions = new StatefullCollisionDetector(Elements);

            TimeDependentActionable gravityAction = new TimeDependentActionable
                (
                    interval =>
                    {
                        var pendingGravityImpulses =
                        from e in Elements
                        select gravity.Act(e, interval);

                        var pendingCollisionImpulses = collisions.Act();

                        var allImpulses = pendingCollisionImpulses
                        //.Concat(pendingGravityImpulses)
                        ;
                        

                        var impulseGroups = allImpulses.GroupBy(pe => pe.Element, pe => pe.Impulse);
                        
                        foreach (var impulseGroup in impulseGroups)
                        {
                            IElement element = impulseGroup.Key;
                            var totalImpulse = impulseGroup.Aggregate((d1, d2) => d1 + d2);

                            System.Diagnostics.Debug.WriteLine(string.Empty);
                            System.Diagnostics.Debug.WriteLine(element.ToString());
                            element.Velocity = new Velocity(element.Velocity.Dimensions + totalImpulse);
                            System.Diagnostics.Debug.WriteLine(element.ToString());
                        }

                    }
                );

            TimeDependentActionable velocityAction = new TimeDependentActionable
                (
                    interval => {
                        foreach (var element in Elements) {
                            element.Location = element.Velocity.Act(element.Location, interval);
                        }
                    }
                );

            
            
            

                
                
                

            
            _boundry = new Boundry(new Dimensions(525, 350), Elements);
                                   
            timeDependentActions = new List<TimeDependentAction>();

            //Move the objects
            timeDependentActions.Add(velocityAction);
            //Calculate the impulses
            
            timeDependentActions.Add(gravityAction);
            //reclaculate the velocity
            //enforce bAOUNDRY
            timeDependentActions.Add(_boundry);








            _cancellationTokenSource = new CancellationTokenSource();
            time = new UniversalTime(timeDependentActions, _cancellationTokenSource.Token);
        }



        private Square NewSquare()
        {
            return new Square(12, new Dimensions(RandomX, RandomY), new Velocity(new Dimensions(10, 20)));
        }

        private Ball NewBall() {
            return new Ball( 8, new Dimensions(RandomX, RandomY), new Velocity(new Dimensions(10, 20)));
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

