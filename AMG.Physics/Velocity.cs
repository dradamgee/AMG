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

            //// todo: do we need this invoke?
            //Universe.Dispatcher.Invoke(new Action(() => {
                _element.Location += Dimensions * interval;
            //}));            
        }

        public void Bounce(Dimensions dimensions)
        {
            var sumOfDimensions = (-Dimensions.X * dimensions.X - Dimensions.Y * dimensions.Y);

            if (sumOfDimensions > 0.0) {
                var impulse = dimensions * sumOfDimensions;
                Dimensions = Dimensions + impulse * 2.0;
            }
        }
    }
} 