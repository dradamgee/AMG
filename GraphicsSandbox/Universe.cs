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
        private readonly double _loss;
        UniversalTime time;
        CancellationTokenSource _cancellationTokenSource;
        private Boundry _boundry;
        List<TimeDependentAction> timeDependentActions;
        private int height;
        Queue<Element> _pendingElementAdds = new Queue<Element>();
        Queue<Element> _pendingElementRemoves = new Queue<Element>();
        Queue<Tuple<Element, ForceViewModel>> _pendingClear = new Queue<Tuple<Element, ForceViewModel>>();
        Queue<ForceViewModel> _pendingBondAdds = new Queue<ForceViewModel>();

        public void Add(Element element)
        {
            element.ExpandCommand = new MyElementCommand(element, this.split);
            element.CollapseCommand = new MyElementCommand(element, this.makeRoot);
            _pendingElementAdds.Enqueue(element);
        }

        public void Add(Element element, Element subnode)
        {
            var bond = new Bond(element, subnode, element.Radius * 2.0, subnode.Mass * 10.0);
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

        private void makeRoot(Element element)
        {
            var leash = new Leash(new Vector(500.0, 10.0), element, element.Radius * 1.1, 10000.0);
            var lvm = new ForceViewModel(leash);
            _pendingClear.Enqueue(Tuple.Create<Element, ForceViewModel>(element, lvm));

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

        public Universe(double accelerationDueToGravity, double loss, double viscosity) {
            _loss = loss;
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            VisualElements = new ObservableCollection<object>();
            elementModels = new List<IElement>();
            bondModels = new List<IForce>();

            _boundry = new Boundry(new Vector(525, 350), elementModels, loss);
            var gravity = new Gravity(accelerationDueToGravity);
            var drag = new Drag(viscosity);
            
            //ICollisionDetector collisions = new PairCollisionDetector(elementModels);
            ICollisionDetector collisions = new QuadTreeCollisionDetector(elementModels, _boundry);
            //ICollisionDetector collisions = new StatefullCollisionDetector(elementModels);
            CollisionResolution collisionResolution = new CollisionResolution(loss);

            var clearAction = new TimeDependentActionable(

                interval =>
                {
                    while (_pendingClear.Any())
                    {
                        _pendingElementAdds.Clear();
                        _pendingElementRemoves.Clear();
                        _pendingBondAdds.Clear();
                        elementModels.Clear();
                        bondModels.Clear();
                        dispatcher.BeginInvoke(new Action(() => VisualElements.Clear()));

                        var item = _pendingClear.Dequeue();
                        var element = item.Item1;
                        var bondVM = item.Item2;

                        elementModels.Add(element);
                        dispatcher.BeginInvoke(new Action(() => VisualElements.Add(element)));
                        bondModels.Add(bondVM.Force);
                        dispatcher.BeginInvoke(new Action(() => VisualElements.Add(bondVM)));
                    }
                }
                
                );

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

                        var pendingDragImpulses =
                            from e in elementModels
                            select drag.Act(e, interval);


                        var pendingBondImpulses = bondModels.SelectMany(b => b.Act(interval));

                        var possibleCollisions = collisions.Detect();
                        var pendingCollisionImpulses = collisionResolution.Act(possibleCollisions);
                        
                        var allImpulses = pendingGravityImpulses
                        .Concat(pendingCollisionImpulses)
                        .Concat(pendingBondImpulses)
                        .Concat(pendingDragImpulses)
                            ;

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

            timeDependentActions.Add(clearAction);
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

