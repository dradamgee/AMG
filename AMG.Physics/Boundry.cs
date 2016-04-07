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

        public override void Act() {
            var d = 0.9;

            ResetInterval();

            foreach (var element in _elements) {
                if (element.Location.X > Dimensions.X)
                {
                    if (element.Velocity.Dimensions.X > 0)
                    {
                        element.Velocity.Dimensions *= new Dimensions(-1, 1) * d;
                    }
                }
                if (element.Location.X < 0) 
                {
                    if (element.Velocity.Dimensions.X < 0) 
                    {
                        element.Velocity.Dimensions *= new Dimensions(-1, 1) * d;
                    }
                }

                if (element.Location.Y > Dimensions.Y) 
                {
                    if (element.Velocity.Dimensions.Y > 0) 
                    {
                        element.Velocity.Dimensions *= new Dimensions(1, -1) * d;
                    }
                }
                if (element.Location.Y < 0) 
                {
                    if (element.Velocity.Dimensions.Y < 0) 
                    {
                        element.Velocity.Dimensions *= new Dimensions(1, -1) * d;
                    }
                }

            }
        }
    }
}