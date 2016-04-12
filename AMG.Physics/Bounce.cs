using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMG.Physics {
    public static class VelocityExtentions {
        public static void Bounce(this Velocity velocity, Dimensions dimensions)
        {
            //var dimensions = impulseDirection.Unit; //  this probably should already be a unit.

            var sumOfDimensions = (-velocity.Dimensions.X*dimensions.X-velocity.Dimensions.Y*dimensions.Y);

            if (sumOfDimensions > 0.0)
            {
                var impulse = dimensions * sumOfDimensions;
                velocity.Dimensions = velocity.Dimensions + impulse * 2.0;
            }
        }
    }
}
