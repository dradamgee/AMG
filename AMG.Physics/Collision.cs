//using System;

//namespace AMG.Physics
//{
//    public class Collision {

//        public static void Act(IElement e1, IElement e2)
//        {
//            var _distance = e1.Radius + e2.Radius;

//            if (e1 == e2)
//            {
//                throw new Exception("Cant collide with self");
//            }

//            var distance = e1.Location - e2.Location;
//            if (distance.Magnitude > _distance)
//            {
//                return;
//            }

//            e1.Velocity.Bounce(distance.Unit * loss);
//            e2.Velocity.Bounce(distance.Unit * -loss);
//        }

//        private static double loss = 0.9;
       
//    }
//}