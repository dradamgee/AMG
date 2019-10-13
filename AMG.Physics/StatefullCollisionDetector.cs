﻿using AMG.FySics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Microsoft.FSharp.Core;

namespace AMG.Physics
{
    public class CollisionResolution
    {
        public CollisionResolution(double loss)
        {
            collision = new Collision(loss);
        }

        private Collision collision;

        public IEnumerable<PendingImpulse> Act(IEnumerable<Tuple<Element, Element>> _pairs) {
            var pairs = _pairs.ToArray();
            foreach (var pair in pairs) {
                var e1 = pair.Item1;
                var e2 = pair.Item2;
                var impulse = collision.Act(e1, e2);
                if (impulse != null) {
                    //System.Diagnostics.Debug.WriteLine(e1.ToString());
                    //System.Diagnostics.Debug.WriteLine(e2.ToString());
                    //System.Diagnostics.Debug.WriteLine(impulse.Value);

                    yield return new PendingImpulse(e1, impulse.Value);
                    yield return new PendingImpulse(e2, -impulse.Value);
                }
            }
        }
    }

    //public class StatefullCollisionDetector : ICollisionDetector
    //{
        
    //    private readonly Element[] _elementsOrderedByX;
    //    private readonly Element[] _elementsOrderedByY;
    //    private readonly int _count;

    //    public StatefullCollisionDetector(IEnumerable<Element> elements)
    //    {
    //        //TODO react to changes in the ObservableCollection

    //        _elementsOrderedByX = elements.ToArray();
    //        _elementsOrderedByY = elements.ToArray();
    //        _count = _elementsOrderedByX.Length;
    //    }

        

    //    public IEnumerable<Tuple<Element, Element>> Detect() {
    //        Array.Sort(_elementsOrderedByX, (e1, e2) => {
    //                                                        return e1.Location.X.CompareTo(e2.Location.X);
    //        });
    //        Array.Sort(_elementsOrderedByY, (e1, e2) => {
    //                                                        return e1.Location.Y.CompareTo(e2.Location.Y);
    //        });

    //        int x1 = 0;
    //        while (x1 < _count)
    //        {
    //            int x2 = x1 + 1;
    //            while (x2 < _count) 
    //            {
    //                var e1 = _elementsOrderedByX[x1];
    //                var e2 = _elementsOrderedByX[x2];
    //                if  (Math.Abs(e1.Location.X - e2.Location.X) <= e1.Radius + e1.Radius) 
    //                {
    //                    if (Math.Abs(e1.Location.Y - e2.Location.Y) <= e1.Radius + e1.Radius)
    //                        yield return new Tuple<Element, Element>(e1, e2);
    //                } 
    //                else
    //                {
    //                    break;
    //                }
    //                x2++;
    //            }

    //            x1++;
    //        }
    //    }
    //}
}