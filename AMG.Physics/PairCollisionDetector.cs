using AMG.FySics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMG.Physics
{
    public class PairCollisionDetector : ICollisionDetector
    {
        private readonly IEnumerable<IElement> _elements;
        public PairCollisionDetector(IEnumerable<IElement> elements)
        {
            _elements = elements;
        }
        public IEnumerable<Tuple<IElement, IElement>> Detect() {

            var elements = _elements.ToArray();

            for (int i = 0; i < elements.Length; i++) {
                for (int j = i + 1; j < elements.Length; j++) {
                    var e1 = elements[i];
                    var e2 = elements[j];

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