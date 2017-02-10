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
       
        public void Add(IElement element)
        {
            Elements.Add(element);
        }

        
        public Universe(double accelerationDueToGravity, double loss) {
            Elements = new ObservableCollection<IElement>();
            _boundry = new Boundry(new Vector(525, 350), Elements);
            var gravity = new Gravity(accelerationDueToGravity);
            var collisions = new PairCollisionDetector(Elements);
            CollisionResolution collisionResolution = new CollisionResolution(loss);
            var gravityAction = new TimeDependentActionable
                (
                    interval =>
                    {
                        var pendingGravityImpulses =
                        from e in Elements
                        select gravity.Act(e, interval);

                        var possibleCollisions = collisions.Detect();
                        var pendingCollisionImpulses = collisionResolution.Act(possibleCollisions);

                        var allImpulses = pendingCollisionImpulses
                        .Concat(pendingGravityImpulses)
                        ;
                        

                        var impulseGroups = allImpulses.GroupBy(pe => pe.Element, pe => pe.Impulse);
                        
                        foreach (var impulseGroup in impulseGroups)
                        {
                            IElement element = impulseGroup.Key;
                            var totalImpulse = impulseGroup.Aggregate((d1, d2) => d1 + d2);

                            //System.Diagnostics.Debug.WriteLine(string.Empty);
                            //System.Diagnostics.Debug.WriteLine(element.ToString());
                            element.Velocity = new Velocity(element.Velocity.Vector + totalImpulse / element.Mass);
                            //System.Diagnostics.Debug.WriteLine(element.ToString());
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

            time.Start();
        }

        public ObservableCollection<IElement> Elements { get; }

        public Vector Size
        {
            get
            {
                return _boundry.Size;
            }
            set
            {
                _boundry.Size = value;
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

