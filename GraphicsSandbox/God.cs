using System;
using AMG.FySics;

namespace GraphicsSandbox
{
    public static class God // not sure if God should be static, or maybe immutable.
    {
        private static double accelerationDueToGravity = 98;
        private static int NumberOfBalls = 10;
        private static int BallSize = 15;
        private static double loss = 0.0;

        public static Universe CreateUniverse() {
            var universe = new Universe(accelerationDueToGravity, loss);
            
            int i = NumberOfBalls;
            while (i-- > 0) {
                universe.Add(NewBall());
            }

            return universe;
        }

        public static  Universe CreateUniverseToFixRotationError() {
            var universe = new Universe(accelerationDueToGravity, loss);
            var e1 = (Ball)"1 | 8 | 196.5716629592 | 225.237471658389 | 4.96841730694 | 5.77965926300911";
            var e2 = (Ball)"1 | 8 | 185.307610553024 | 214.62117119859 | 15.1709404889631 | -5.65428572085577";
            universe.Add(e1);
            universe.Add(e2);
            return universe;
        }

        
        private static Square NewSquare() {
            return new Square(1.0d, 12, new Vector(RandomX, RandomY), new Velocity(new Vector(10, 20)));
        }

        private static Ball NewBall() {
            return new Ball(1.0d, BallSize, new Vector(RandomX, RandomY), new Velocity(new Vector(10, 20)));
        }

        static Random random = new Random();

        private static int RandomX
        {
            get { return random.Next(525); }
        }
        private static int RandomY
        {
            get { return random.Next(350); }
        }
    }
}