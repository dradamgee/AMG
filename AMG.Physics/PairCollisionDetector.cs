using AMG.FySics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMG.Physics
{
    public class PairCollisionDetector : TimeDependentAction, ICollisionDetector
    {
        private Collision collision = new Collision(0.9);
        private readonly IElement[] _elements;

        public PairCollisionDetector(IEnumerable<IElement> elements)
        {
            _elements = elements.ToArray();
        }

        public override void Act(double interval)
        {
            var pairs = Detect();
            foreach (var pair in pairs)
            {
                collision.Act(pair.Item1, pair.Item2);
            }
        }


        public IEnumerable<Tuple<IElement, IElement>> Detect() {

            for (int i = 0; i < _elements.Length; i++) {
                for (int j = i + 1; j < _elements.Length; j++) {
                    var e1 = _elements[i];
                    var e2 = _elements[j];

                    if (Math.Abs(e1.Location.X - e2.Location.X) > e1.Radius + e1.Radius)
                        continue;
                    if (Math.Abs(e1.Location.Y - e2.Location.Y) > e1.Radius + e1.Radius)
                        continue;
                    
                    yield return new Tuple<IElement, IElement>(e1, e2);
                }
            }
        }
    }
}