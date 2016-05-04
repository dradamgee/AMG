using AMG.FySics;
using System.Collections.Generic;

namespace AMG.Physics
{
    public class Boundry : TimeDependentAction
    {
        public Dimensions Dimensions;
        
        private readonly IEnumerable<IElement> _elements;

        public Boundry(Dimensions dimensions, IEnumerable<IElement> elements)
        {
            _elements = elements;
            Dimensions = dimensions;
        }


        static double d = 1.0;
        Unit XUp = new Unit(d, 0.0);
        Unit XDown = new Unit(-d, 0.0);
        Unit YUp = new Unit(0.0, d);
        Unit YDown = new Unit(0.0, -d);

        public override void Act(double interval) {
            //ResetInterval();

            foreach (var element in _elements) {
                if (element.Location.X > Dimensions.X)
                {
                    if (element.Velocity.Dimensions.X > 0)
                    {
                        element.Velocity = new Velocity(element.Velocity.Bounce(XDown));
                    }
                }
                if (element.Location.X < 0) 
                {
                    if (element.Velocity.Dimensions.X < 0) 
                    {
                        element.Velocity = new Velocity(element.Velocity.Bounce(XUp));
                    }
                }

                if (element.Location.Y > Dimensions.Y) 
                {
                    if (element.Velocity.Dimensions.Y > 0) 
                    {
                        element.Velocity = new Velocity(element.Velocity.Bounce(YDown));
                    }
                }
                if (element.Location.Y < 0) 
                {
                    if (element.Velocity.Dimensions.Y < 0) 
                    {
                        element.Velocity = new Velocity(element.Velocity.Bounce(YUp));
                    }
                }

            }
        }
    }
}