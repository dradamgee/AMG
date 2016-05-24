using AMG.FySics;
using System.Collections.Generic;

namespace AMG.Physics
{
    public class Boundry : TimeDependentAction
    {
        public Vector Size;
        
        private readonly IEnumerable<IElement> _elements;

        public Boundry(Vector size, IEnumerable<IElement> elements)
        {
            _elements = elements;
            Size = size;
        }


        static double d = 1.0;
        Unit XUp = new Unit(d, 0.0);
        Unit XDown = new Unit(-d, 0.0);
        Unit YUp = new Unit(0.0, d);
        Unit YDown = new Unit(0.0, -d);

        public override void Act(double interval) {
            //ResetInterval();

            foreach (var element in _elements) {
                if (element.Right > Size.X)
                {
                    if (element.Velocity.Vector.X > 0)
                    {
                        element.Velocity = new Velocity(element.Velocity.Bounce(XDown));
                    }
                }
                if (element.Left < 0) 
                {
                    if (element.Velocity.Vector.X < 0) 
                    {
                        element.Velocity = new Velocity(element.Velocity.Bounce(XUp));
                    }
                }

                if (element.Bottom > Size.Y) 
                {
                    if (element.Velocity.Vector.Y > 0) 
                    {
                        element.Velocity = new Velocity(element.Velocity.Bounce(YDown));
                    }
                }
                if (element.Top < 0) 
                {
                    if (element.Velocity.Vector.Y < 0) 
                    {
                        element.Velocity = new Velocity(element.Velocity.Bounce(YUp));
                    }
                }

            }
        }
    }
}