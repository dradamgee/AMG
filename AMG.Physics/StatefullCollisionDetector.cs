using AMG.FySics;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.FSharp.Core;

namespace AMG.Physics
{
    public class StatefullCollisionDetector : ICollisionDetector
    {
        private Collision collision = new Collision(1.0);
        private readonly IElement[] _elementsOrderedByX;
        private readonly IElement[] _elementsOrderedByY;
        private readonly int _count;

        public StatefullCollisionDetector(IEnumerable<IElement> elements)
        {
            _elementsOrderedByX = elements.ToArray();
            _elementsOrderedByY = elements.ToArray();
            _count = _elementsOrderedByX.Length;
        }

        public IEnumerable<PendingImpulse> Act()
        {
            var pairs = Detect().ToArray();
            foreach (var pair in pairs) {
                var e1 = pair.Item1;
                var e2 = pair.Item2;
                var impulse = collision.Act(e1, e2);
                if (impulse != null)
                {
                    //System.Diagnostics.Debug.WriteLine(e1.ToString());
                    //System.Diagnostics.Debug.WriteLine(e2.ToString());
                    //System.Diagnostics.Debug.WriteLine(impulse.Value);

                    yield return new PendingImpulse(e1, impulse.Value);
                    yield return new PendingImpulse(e2, -impulse.Value);                    
                }
            }
        }

        public IEnumerable<Tuple<IElement, IElement>> Detect() {
            Array.Sort(_elementsOrderedByX, (e1, e2) => {
                                                            return e1.Location.X.CompareTo(e2.Location.X);
            });
            Array.Sort(_elementsOrderedByY, (e1, e2) => {
                                                            return e1.Location.Y.CompareTo(e2.Location.Y);
            });

            int x1 = 0;
            while (x1 < _count)
            {
                int x2 = x1 + 1;
                while (x2 < _count) 
                {
                    var e1 = _elementsOrderedByX[x1];
                    var e2 = _elementsOrderedByX[x2];
                    if  (Math.Abs(e1.Location.X - e2.Location.X) <= e1.Radius + e1.Radius) 
                    {
                        if (Math.Abs(e1.Location.Y - e2.Location.Y) <= e1.Radius + e1.Radius)
                            yield return new Tuple<IElement, IElement>(e1, e2);
                    } 
                    else
                    {
                        break;
                    }
                    x2++;
                }

                x1++;
            }
        }
    }
}