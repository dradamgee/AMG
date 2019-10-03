using AMG.FySics;
using System.Collections.Generic;

namespace AMG.Physics
{
    public class Boundry : ITimeDependentAction
    {
        public Vector Size;
        
        private readonly IEnumerable<IElement> _elements;
        private readonly double _loss;

        public Boundry(Vector size, IEnumerable<IElement> elements, double loss)
        {
            _elements = elements;
            _loss = loss;
            Size = size;
        }


        static double d = 1.0; // TODO here we appear to be able to create a unit without a magnitude of 1
        Unit XUp = new Unit(d, 0.0);
        Unit XDown = new Unit(-d, 0.0);
        Unit YUp = new Unit(0.0, d);
        Unit YDown = new Unit(0.0, -d);

        public void Act(double interval) {
            //ResetInterval();

            foreach (var element in _elements) {
                if (element.Right > Size.X)
                {
                    if (element.Velocity.Vector.X > 0)
                    {
                        element.Velocity = new Velocity(element.Velocity.Bounce(XDown, _loss));
                    }
                }
                if (element.Left < 0) 
                {
                    if (element.Velocity.Vector.X < 0) 
                    {
                        element.Velocity = new Velocity(element.Velocity.Bounce(XUp, _loss));
                    }
                }

                if (element.Bottom > Size.Y) 
                {
                    if (element.Velocity.Vector.Y > 0) 
                    {
                        element.Velocity = new Velocity(element.Velocity.Bounce(YDown, _loss));
                    }
                }
                if (element.Top < 0) 
                {
                    if (element.Velocity.Vector.Y < 0) 
                    {
                        element.Velocity = new Velocity(element.Velocity.Bounce(YUp, _loss));
                    }
                }

            }
        }
    }
}