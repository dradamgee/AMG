﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AMG.Physics
{
    public class StatefullCollisionDetector : TimeDependentAction, ICollisionDetector
    {
        private readonly IElement[] _elementsOrderedByX;
        private readonly IElement[] _elementsOrderedByY;
        private readonly int _count;

        public StatefullCollisionDetector(IEnumerable<IElement> elements)
        {
            _elementsOrderedByX = elements.ToArray();
            _elementsOrderedByY = elements.ToArray();
            _count = _elementsOrderedByX.Length;
        }

        public override void Act()
        {
            var pairs = Detect().ToArray();
            foreach (var pair in pairs) {
                Collision.Act(pair.Item1, pair.Item2);
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