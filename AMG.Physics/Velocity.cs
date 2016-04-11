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

        public void Bounce(Dimensions direction)
        {
            if (direction.X != 0.0 && Dimensions.X != 0.0)
            {
                var rescaledDirection = direction * -Dimensions.X / direction.X;
                var impulse = Dimensions.Y + rescaledDirection.Y;


                Dimensions = (-1.0 * Dimensions) + new Dimensions(impulse, impulse);

            }
            else if (direction.Y != 0.0 && Dimensions.Y != 0.0)
            {
                var rescaledDirection = direction * -Dimensions.Y / direction.Y;
                var impulseX = Dimensions + rescaledDirection;
                
                Dimensions = (-1.0 * Dimensions) + impulseX + impulseY;
            }
        }
    }
} 