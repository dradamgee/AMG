using System;
using System.Diagnostics;

namespace GraphicsSandbox
{


    public class Velocity : TimeDependentAction {
        private readonly Element _element;

        public Velocity(Element element)
        {
            _element = element;
        }

        public Dimensions Dimensions = new Dimensions(0, 0);

        public override void Act()
        {
            double interval = ResetInterval();

            // todo: do we need this invoke?
            Universe.Dispatcher.Invoke(new Action(() => {
                _element.Location += Dimensions * interval;
            }));
            
        }
    }
}