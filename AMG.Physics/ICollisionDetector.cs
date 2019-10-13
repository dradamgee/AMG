using AMG.FySics;
using System;
using System.Collections.Generic;

namespace AMG.Physics
{
    public interface ICollisionDetector
    {
        IEnumerable<Tuple<Element, Element>> Detect(IEnumerable<Element> elements);
    }
}