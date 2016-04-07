using System.Collections.Generic;
using System.Diagnostics;

namespace GraphicsSandbox
{
    public class Gravity : TimeDependentAction
    {
        private readonly Dimensions _acceleration;
        private readonly IEnumerable<Element> _elements;

        public Gravity(double acceleration, IEnumerable<Element> elements) {
            _acceleration = new Dimensions(0d, acceleration);
            _elements = elements;
        }


        public override void Act() 
        {
            var resetInterval = ResetInterval();
            
            foreach (var element in _elements)
            {
                element.Velocity.Dimensions += _acceleration * resetInterval;
            }
        }
    }
}