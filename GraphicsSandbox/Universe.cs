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
        Queue<Element> _pendingElementAdds = new Queue<Element>();
        Queue<Element> _pendingElementRemoves = new Queue<Element>();
        Queue<ForceViewModel> _pendingBondAdds = new Queue<ForceViewModel>();

        public void Add(Element element)
        {
            element.ElementCommand = new MyElementCommand(element, this.split);
            _pendingElementAdds.Enqueue(element);
        }

        public void Add(Element element, Element subnode)
        {
            var bond = new Bond(element, subnode, 100.0, 100.0);
            var BondVM = new ForceViewModel(bond);
            _pendingBondAdds.Enqueue(BondVM);
        }

        public void Add(Bond bond)
        {
            _pendingBondAdds.Enqueue(new ForceViewModel(bond));
        }

        public void Add(Leash leash)
        {
            _pendingBondAdds.Enqueue(new ForceViewModel(leash));
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

        public void Remove(Element element)
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
            VisualElements = new ObservableCollection<object>();
            elementModels = new List<IElement>();
            bondModels = new List<IForce>();

            _boundry = new Boundry(new Vector(525, 350), elementModels, loss);
            var gravity = new Gravity(accelerationDueToGravity);
            //ICollisionDetector collisions = new PairCollisionDetector(elementModels);
            ICollisionDetector collisions = new QuadTreeCollisionDetector(elementModels, _boundry);
            //ICollisionDetector collisions = new StatefullCollisionDetector(elementModels);
            CollisionResolution collisionResolution = new CollisionResolution(loss);

            var addAction = new TimeDependentActionable
                (
                    interval =>
                    {
                        
                        while (_pendingElementAdds.Any())
                        {
                            var element = _pendingElementAdds.Dequeue();
                            elementModels.Add(element);
                            dispatcher.BeginInvoke(new Action(() => VisualElements.Add(element)));
                        }

                        while (_pendingBondAdds.Any())
                        {
                            var bondVM = _pendingBondAdds.Dequeue();
                            bondModels.Add(bondVM.Force);
                            dispatcher.BeginInvoke(new Action(() => VisualElements.Add(bondVM)));
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
                        elementModels.Remove(element);
                        dispatcher.BeginInvoke(new Action(() => VisualElements.Remove(element)));
                    }
                }
            );



            var impulseAction = new TimeDependentActionable
                (
                    interval =>
                    {
                        var pendingGravityImpulses =
                        from e in elementModels
                        select gravity.Act(e, interval);

                        var pendingBondImpulses = bondModels.SelectMany(b => b.Act(interval));

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
                        foreach (var element in elementModels) {
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

        public ObservableCollection<Object> VisualElements { get; }
        private List<IElement> elementModels { get; }
        private List<IForce> bondModels{ get; }


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

