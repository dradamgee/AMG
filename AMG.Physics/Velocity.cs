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
    }
} 