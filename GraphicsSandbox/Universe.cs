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
                //System.Diagnostics.Debug.WriteLine(elementViewModel.ToString());
                element.Velocity = new Velocity(element.Velocity.Vector + totalImpulse / element.Mass);
                //System.Diagnostics.Debug.WriteLine(elementViewModel.ToString());
            }
        }
    }

    public class Universe : INotifyPropertyChanged, IDisposable
    {
        private readonly double _loss;
        UniversalTime time;
        CancellationTokenSource _cancellationTokenSource;
        private Boundry _boundry;
        List<ITimeDependentAction> timeDependentActions;
        private int height;
        Queue<ElementViewModel> _pendingElementAdds = new Queue<ElementViewModel>();
        Queue<ElementViewModel> _pendingElementRemoves = new Queue<ElementViewModel>();
        Queue<Tuple<ElementViewModel, ForceViewModel>> _pendingClear = new Queue<Tuple<ElementViewModel, ForceViewModel>>();
        Queue<ForceViewModel> _pendingBondAdds = new Queue<ForceViewModel>();

        public void Add(ElementViewModel elementViewModel)
        {
            elementViewModel.ExpandCommand = new MyElementCommand(elementViewModel, this.split);
            elementViewModel.CollapseCommand = new MyElementCommand(elementViewModel, this.makeRoot);
            _pendingElementAdds.Enqueue(elementViewModel);
        }

        public void Add(ElementViewModel elementViewModel, ElementViewModel subnode)
        {
            var bond = new Bond(elementViewModel, subnode, elementViewModel.Radius * 2.0, subnode.Mass * 10.0);
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

        private void split(ElementViewModel elementViewModel)
        {
            var subNodes = elementViewModel.Split();
            foreach (ElementViewModel subnode in subNodes)
            {
                Add(elementViewModel, subnode);
                Add(subnode);
            }
        }

        private void makeRoot(ElementViewModel elementViewModel)
        {
            var leash = new Leash(new Vector(500.0, 10.0), elementViewModel, elementViewModel.Radius * 1.1, 10000.0);
            var lvm = new ForceViewModel(leash);
            _pendingClear.Enqueue(Tuple.Create<ElementViewModel, ForceViewModel>(elementViewModel, lvm));

        }

        public void Remove(ElementViewModel elementViewModel)
        {
            _pendingElementRemoves.Enqueue(elementViewModel);
        }

        public void update(IEnumerable<IElement> elementToRemove, IEnumerable<IElement> elementToAdd)
        {
            foreach (ElementViewModel element in elementToRemove)
            {
                Remove(element);
            }
            foreach (ElementViewModel element in elementToAdd)
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

            timeDependentActions = new List<ITimeDependentAction>();

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

