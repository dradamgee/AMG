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
    //TODO move this to FySics
    public class Dynamics
    {
        public static void ProcessImpulses(IEnumerable<PendingImpulse> allImpulses) {
            var impulseGroups = allImpulses.GroupBy(pe => pe.Element, pe => pe.Impulse);

            foreach (var impulseGroup in impulseGroups) {
                IElement element = impulseGroup.Key;
                var totalImpulse = impulseGroup.Aggregate((d1, d2) => d1 + d2);

                //System.Diagnostics.Debug.WriteLine(string.Empty);
                //System.Diagnostics.Debug.WriteLine(element.ToString());
                element.Velocity = new Velocity(element.Velocity.Vector + totalImpulse / element.Mass);
                //System.Diagnostics.Debug.WriteLine(element.ToString());
            }
        }
    }


    public class Universe : INotifyPropertyChanged, IDisposable
    {   
        UniversalTime time;
        CancellationTokenSource _cancellationTokenSource;
        private Boundry _boundry;
        List<TimeDependentAction> timeDependentActions;
        private int height;
        Queue<IElement> pendingAdds = new Queue<IElement>();
       
        public void Add(IElement element)
        {
            pendingAdds.Enqueue(element);
        }
        
        
        public Universe(double accelerationDueToGravity, double loss) {
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            Elements = new ObservableCollection<IElement>();
            internalElements = new List<IElement>();
            
            _boundry = new Boundry(new Vector(525, 350), internalElements);
            var gravity = new Gravity(accelerationDueToGravity);
            //ICollisionDetector collisions = new PairCollisionDetector(internalElements);
            ICollisionDetector collisions = new QuadTreeCollisionDetector(internalElements, _boundry);
            //ICollisionDetector collisions = new StatefullCollisionDetector(internalElements);
            CollisionResolution collisionResolution = new CollisionResolution(loss);

            var addAction = new TimeDependentActionable
                (
                    interval =>
                    {
                        
                        while (pendingAdds.Any())
                        {
                            var element = pendingAdds.Dequeue();
                            internalElements.Add(element);
                            dispatcher.BeginInvoke(new Action(() => Elements.Add(element)));
                        }
                    }
                );
            

            var impulseAction = new TimeDependentActionable
                (
                    interval =>
                    {
                        var pendingGravityImpulses =
                        from e in internalElements
                        select gravity.Act(e, interval);

                        var possibleCollisions = collisions.Detect();
                        var pendingCollisionImpulses = collisionResolution.Act(possibleCollisions);

                        var allImpulses = pendingCollisionImpulses
                        .Concat(pendingGravityImpulses)
                        ;
                        Dynamics.ProcessImpulses(allImpulses);
                    }
                );

            TimeDependentActionable velocityAction = new TimeDependentActionable
                (
                    interval => {
                        foreach (var element in internalElements) {
                            element.Location = element.Velocity.Act(element.Location, interval);
                        }
                    }
                );

            timeDependentActions = new List<TimeDependentAction>();

            timeDependentActions.Add(addAction);

            //Move the objects
            timeDependentActions.Add(velocityAction);
            //Calculate the impulses
            
            timeDependentActions.Add(impulseAction);
            //reclaculate the velocity
            //enforce boundry
            timeDependentActions.Add(_boundry);
            
            _cancellationTokenSource = new CancellationTokenSource();
            time = new UniversalTime(timeDependentActions, _cancellationTokenSource.Token);

            time.Start();
        }



        public ObservableCollection<IElement> Elements { get; }
        private List<IElement> internalElements { get; }


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

