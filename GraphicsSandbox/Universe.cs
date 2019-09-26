using AMG.FySics;
using AMG.Physics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Media;
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
        Queue<IElement> _pendingElementAdds = new Queue<IElement>();
        Queue<IElement> _pendingElementRemoves = new Queue<IElement>();
        Queue<Bond> _pendingBondAdds = new Queue<Bond>();

        public void Add(Element element)
        {
            element.ElementCommand = new MyElementCommand(element, this.split);
            _pendingElementAdds.Enqueue(element);
        }

        public void Add(Element element, Element subnode)
        {
            _pendingBondAdds.Enqueue(new Bond(element, subnode, 100.0, 100.0));
        }

        private void split(Element element)
        {
            var subNodes = element.Split();
            foreach (Element subnode in subNodes)
            {
                Add(element, subnode);
                Add(subnode);
            }
        }

        public void Remove(IElement element)
        {
            _pendingElementRemoves.Enqueue(element);
        }

        public void update(IEnumerable<IElement> elementToRemove, IEnumerable<IElement> elementToAdd)
        {
            foreach (Element element in elementToRemove)
            {
                Remove(element);
            }
            foreach (Element element in elementToAdd)
            {
                Add(element);
            }
        }

        public Universe(double accelerationDueToGravity, double loss) {
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            Elements = new ObservableCollection<object>();
            internalElements = new List<IElement>();
            internalBonds = new List<Bond>();

            _boundry = new Boundry(new Vector(525, 350), internalElements, loss);
            var gravity = new Gravity(accelerationDueToGravity);
            //ICollisionDetector collisions = new PairCollisionDetector(internalElements);
            ICollisionDetector collisions = new QuadTreeCollisionDetector(internalElements, _boundry);
            //ICollisionDetector collisions = new StatefullCollisionDetector(internalElements);
            CollisionResolution collisionResolution = new CollisionResolution(loss);

            var addAction = new TimeDependentActionable
                (
                    interval =>
                    {
                        
                        while (_pendingElementAdds.Any())
                        {
                            var element = _pendingElementAdds.Dequeue();
                            internalElements.Add(element);
                            dispatcher.BeginInvoke(new Action(() => Elements.Add(element)));
                        }

                        while (_pendingBondAdds.Any())
                        {
                            var bond = _pendingBondAdds.Dequeue();
                            internalBonds.Add(bond);
                            var vm = new BondViewModel(bond);
                            dispatcher.BeginInvoke(new Action(() => Elements.Add(vm)));
                        }
                    }
                );

            var removeAction = new TimeDependentActionable
            (
                interval =>
                {

                    while (_pendingElementRemoves.Any())
                    {
                        var element = _pendingElementRemoves.Dequeue();
                        internalElements.Remove(element);
                        dispatcher.BeginInvoke(new Action(() => Elements.Remove(element)));
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

                        var pendingBondImpulses = internalBonds.SelectMany(b => b.Act(interval));

                        var possibleCollisions = collisions.Detect();
                        var pendingCollisionImpulses = collisionResolution.Act(possibleCollisions);
                        
                        var allImpulses = pendingGravityImpulses
                        .Concat(pendingCollisionImpulses)
                        .Concat(pendingBondImpulses);

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
            timeDependentActions.Add(removeAction);

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



        public ObservableCollection<Object> Elements { get; }
        private List<IElement> internalElements { get; }
        private List<Bond> internalBonds{ get; }


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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose() 
        {
            _cancellationTokenSource.Cancel();
        }
    }
}

