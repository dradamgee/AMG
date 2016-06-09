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
        private int BallSize = 20;
        private int NumberOfBalls = 50;
        private double accelerationDueToGravity = 98;
        private double loss = 1;

        public Universe() {
            Elements = new ObservableCollection<IElement>();



            _boundry = new Boundry(new Vector(525, 350), Elements);

            //var e1 = (Ball)"1 | 8 | 196.5716629592 | 225.237471658389 | 4.96841730694 | 5.77965926300911";
            //var e2 = (Ball)"1 | 8 | 185.307610553024 | 214.62117119859 | 15.1709404889631 | -5.65428572085577";


            var e1 = (Ball)"1 | 20 | 196.5716629592 | 225.237471658389 | 4.96841730694 | 5.77965926300911";

            //Elements.Add(e1);
            //Elements.Add(e2);
            
            int i = NumberOfBalls;
            while (i-- > 0)
            {
                Elements.Add(NewBall());
            }
            
            var gravity = new Gravity(accelerationDueToGravity);
            var collisions = new StatefullCollisionDetector(Elements);
            //var collisions = new QuadTreeCollisionDetector(Elements, _boundry);
            CollisionReolution collisionReolution = new CollisionReolution(loss);
            
            var gravityAction = new TimeDependentActionable
                (
                    interval =>
                    {
                        var pendingGravityImpulses =
                        from e in Elements
                        select gravity.Act(e, interval);

                        var possibleCollisions = collisions.Detect();
                        var pendingCollisionImpulses = collisionReolution.Act(possibleCollisions);

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
        }



        private Square NewSquare()
        {
            return new Square(1.0d, 12, new Vector(RandomX, RandomY), new Velocity(new Vector(10, 20)));
        }

        private Ball NewBall() {
            return new Ball(1.0d, BallSize, new Vector(RandomX, RandomY), new Velocity(new Vector(10, 20)));
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

