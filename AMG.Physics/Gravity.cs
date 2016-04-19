//using System.Collections.Generic;

//namespace AMG.Physics
//{
//    public class Gravity : TimeDependentAction
//    {
//        private readonly Dimensions _acceleration;
//        private readonly IEnumerable<IElement> _elements;

//        public Gravity(double acceleration, IEnumerable<IElement> elements) {
//            _acceleration = new Dimensions(0d, acceleration);
//            _elements = elements;
//        }


//        public override void Act() 
//        {
//            var resetInterval = ResetInterval();
            
//            foreach (var element in _elements)
//            {
//                element.Velocity.Dimensions += _acceleration * resetInterval;
//            }
//        }
//    }
//}