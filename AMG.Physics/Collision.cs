using System;

namespace AMG.Physics
{

    public class Collision : TimeDependentAction {
        private readonly double _distance;
        private readonly IElement _e1;
        private readonly IElement _e2;

        public Collision(double distance, IElement e1, IElement e2)
        {
            _distance = distance;
            _e1 = e1;
            _e2 = e2;
        }

        public override void Act()
        {
            if (_e1 == _e2)
            {
                throw new Exception("Cant collide with self");
            }

            var distance = _e1.Location - _e2.Location;
            if ( distance.Magnitude > _distance ) {
                return;
            }


            _e2.Velocity.Bounce(distance.Unit);
            _e1.Velocity.Bounce(-1.0 * distance.Unit);

        }
    }
}