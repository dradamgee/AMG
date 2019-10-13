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
        public static IEnumerable<PendingImpulse> ProcessImpulses(IEnumerable<PendingImpulse> allImpulses) {
            var impulseGroups = allImpulses.GroupBy(pe => pe.Element, pe => pe.Impulse);



            foreach (var impulseGroup in impulseGroups) {
                Element element = impulseGroup.Key;
                var totalImpulse = impulseGroup.Aggregate((d1, d2) => d1 + d2);

                yield return new PendingImpulse(element, totalImpulse);

                //System.Diagnostics.Debug.WriteLine(string.Empty);
                //System.Diagnostics.Debug.WriteLine(elementViewModel.ToString());


                //element.Velocity = new Velocity(element.Velocity.Vector + totalImpulse / element.Mass);

                //TODO Tick elements

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
        List<TimeDependentActionable> timeDependentActions;
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
            var bond = new Bond(elementViewModel.Radius * 2.0, subnode.Mass * 10.0);
            var BondVM = new BondViewModel(bond, elementViewModel, subnode);
            _pendingBondAdds.Enqueue(BondVM);
        }

        public void Add(BondViewModel bondVM)
        {
            _pendingBondAdds.Enqueue(bondVM);
        }

        public void Add(LeashViewModel leashVM)
        {
            _pendingBondAdds.Enqueue(leashVM);
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
            var leash = new Leash(new Vector(500.0, 10.0), elementViewModel.Radius * 1.1, 10000.0);
            var lvm = new LeashViewModel(leash, elementViewModel);
            _pendingClear.Enqueue(Tuple.Create<ElementViewModel, ForceViewModel>(elementViewModel, lvm));
        }

        public void Remove(ElementViewModel elementViewModel)
        {
            _pendingElementRemoves.Enqueue(elementViewModel);
        }

        public void update(IEnumerable<ElementViewModel> elementToRemove, IEnumerable<ElementViewModel> elementToAdd)
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
            elementViewModels = new Dictionary<int, ElementViewModel>();
            bondViewModels = new List<ForceViewModel>();

            _boundry = new Boundry(new Vector(525, 350), loss);
            var gravity = new Gravity(accelerationDueToGravity);
            var drag = new Drag(viscosity);
            
            ICollisionDetector collisions = new PairCollisionDetector();
            //ICollisionDetector collisions = new QuadTreeCollisionDetector(elementViewModels, _boundry);
            //ICollisionDetector collisions = new StatefullCollisionDetector(elementViewModels);
            CollisionResolution collisionResolution = new CollisionResolution(loss);

            var clearAction = new TimeDependentActionable(

                interval =>
                {
                    while (_pendingClear.Any())
                    {
                        _pendingElementAdds.Clear();
                        _pendingElementRemoves.Clear();
                        _pendingBondAdds.Clear();
                        elementViewModels.Clear();
                        bondViewModels.Clear();
                        dispatcher.BeginInvoke(new Action(() => VisualElements.Clear()));

                        var item = _pendingClear.Dequeue();
                        var element = item.Item1;
                        var bondVM = item.Item2;

                        elementViewModels.Add(element.Id, element);
                        dispatcher.BeginInvoke(new Action(() => VisualElements.Add(element)));
                        bondViewModels.Add(bondVM);
                        dispatcher.BeginInvoke(new Action(() => VisualElements.Add(bondVM)));
                    }
                }
                
                );

            var addAction = new TimeDependentActionable(
                    interval =>
                    {
                        
                        while (_pendingElementAdds.Any())
                        {
                            var element = _pendingElementAdds.Dequeue();
                            elementViewModels.Add(element.Id, element);
                            dispatcher.BeginInvoke(new Action(() => VisualElements.Add(element)));
                        }

                        while (_pendingBondAdds.Any())
                        {
                            var bondVM = _pendingBondAdds.Dequeue();
                            bondViewModels.Add(bondVM);
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
                        elementViewModels.Remove(element.Id);
                        dispatcher.BeginInvoke(new Action(() => VisualElements.Remove(element)));
                    }
                }
            );



            var impulseAction = new TimeDependentActionable
                (
                    interval =>
                    {
                        var elements = elementViewModels.Values.Select(vm => vm.Element).ToList();

                        var pendingGravityImpulses = ((ITimeDependentAction)gravity).Act(interval, elements);
                        var pendingBoundryImpulses = ((ITimeDependentAction)_boundry).Act(loss, elements);
                        var pendingDragImpulses = ((ITimeDependentAction)drag).Act(interval, elements);

                        var pendingBondImpulses = bondViewModels.SelectMany(b => b.Act(interval));

                        var possibleCollisions = collisions.Detect(elementViewModels.Values.Select(vm => vm.Element));
                        var pendingCollisionImpulses = collisionResolution.Act(possibleCollisions);
                        
                        var allImpulses = pendingGravityImpulses
                        .Concat(pendingBoundryImpulses)
                        .Concat(pendingCollisionImpulses)
                        .Concat(pendingBondImpulses)
                        .Concat(pendingDragImpulses)
                            ;

                        var pendingImpulses = Dynamics.ProcessImpulses(allImpulses);

                        foreach (var pendingImpulse in pendingImpulses)
                        {
                            var element = pendingImpulse.Element;
                            this.elementViewModels[element.Id].Element = Time.Tick(interval, element, pendingImpulse);
                        }
                    }
                );

            timeDependentActions = new List<TimeDependentActionable>();

            timeDependentActions.Add(clearAction);
            timeDependentActions.Add(addAction);
            timeDependentActions.Add(removeAction);

            timeDependentActions.Add(impulseAction);
            
            _cancellationTokenSource = new CancellationTokenSource();
            time = new UniversalTime(timeDependentActions, _cancellationTokenSource.Token);

            time.Start();
        }

        public ObservableCollection<Object> VisualElements { get; }
        private Dictionary<int, ElementViewModel> elementViewModels { get; }
        private List<ForceViewModel> bondViewModels{ get; }


        public Vector Size
        {
            get
            {
                return _boundry.Size;
            }
            set
            {
                _boundry = new Boundry(value, _loss);
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

