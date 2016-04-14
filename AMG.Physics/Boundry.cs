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


        static double d = 0.99;
        Dimensions XUp = new Dimensions(d, 0.0);
        Dimensions XDown = new Dimensions(-d, 0.0);
        Dimensions YUp = new Dimensions(0.0, d);
        Dimensions YDown = new Dimensions(0.0, -d);


        public override void Act() {
            ResetInterval();

            foreach (var element in _elements) {
                if (element.Location.X > Dimensions.X)
                {
                    if (element.Velocity.Dimensions.X > 0)
                    {
                        element.Velocity.Bounce(XDown);
                    }
                }
                if (element.Location.X < 0) 
                {
                    if (element.Velocity.Dimensions.X < 0) 
                    {
                        element.Velocity.Bounce(XUp);
                    }
                }

                if (element.Location.Y > Dimensions.Y) 
                {
                    if (element.Velocity.Dimensions.Y > 0) 
                    {
                        element.Velocity.Bounce(YDown);
                    }
                }
                if (element.Location.Y < 0) 
                {
                    if (element.Velocity.Dimensions.Y < 0) 
                    {
                        element.Velocity.Bounce(YUp);
                    }
                }

            }
        }
    }
}