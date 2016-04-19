//using System;

//namespace AMG.Physics
//{


//    public class Velocity : TimeDependentAction {
//        private readonly IElement _element;

//        public Velocity(IElement element)
//        {
//            _element = element;
//        }

//        public Dimensions Dimensions = new Dimensions(0, 0);

//        public override void Act()
//        {
//            double interval = ResetInterval();
//            _element.Location += Dimensions * interval;
//        }

//        public void Bounce(Dimensions dimensions)
//        {
//            var impulse = (-Dimensions.X * dimensions.X - Dimensions.Y * dimensions.Y);

//            if (impulse > 0.0)
//            {
//                var impulseVector = dimensions * impulse;
//                Dimensions = Dimensions + impulseVector * 2.0;
//            }
//        }
//    }
//} 