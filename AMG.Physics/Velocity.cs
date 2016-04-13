using System;

namespace AMG.Physics
{


    public class Velocity : TimeDependentAction {
        private readonly IElement _element;

        public Velocity(IElement element)
        {
            _element = element;
        }

        public Dimensions Dimensions = new Dimensions(0, 0);

        public override void Act()
        {
            double interval = ResetInterval();
            _element.Location += Dimensions * interval;
        }

        public void Bounce(Dimensions direction)
        {
            var impulse = (-Dimensions.X * direction.X - Dimensions.Y * direction.Y);

            if (impulse > 0.0)
            {
                var impulseVector = direction * impulse;
                Dimensions = Dimensions + impulseVector * 2.0;
            }
        }
    }
} 