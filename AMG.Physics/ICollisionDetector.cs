using System;
using System.Collections.Generic;

namespace AMG.Physics
{
    public interface ICollisionDetector
    {
        IEnumerable<Tuple<IElement, IElement>> Detect();
    }
}