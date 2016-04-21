using AMG.FySics;
using NUnit.Framework;

//using static NUnit.Framework.Assert;
//using static System.Math;


namespace AMG.Physics.Test
{
    // I'm guessing these tests are going to fail as Bounce returns an impulse.
    [TestFixture]
    public class TestVelocity : TestAsserts
    {
        [Test]
        public void BounceAt180()
        {
            var v1 = new Velocity(new Dimensions(0.0, -99.0)) ;
            var result = v1.Bounce(new Dimensions(0, 1).Unit);

            AreClose(new Dimensions(0.0, 99.0), result);
        }

        [Test]
        public void BounceAt45()
        {
            var v1 = new Velocity(new Dimensions(0.0, -99.0));
            var result = v1.Bounce(new Dimensions(1, 1).Unit);

            AreClose(new Dimensions(99.0, 0.0), result);          
        }


        public Dimensions Bounce(Dimensions Dimensions, Dimensions dimensions) {
            var impulse = (-Dimensions.X * dimensions.X - Dimensions.Y * dimensions.Y);

            if (impulse > 0.0) {
                var impulseVector = dimensions * impulse;
                return Dimensions + impulseVector * 2.0;
            }
            return null;
        }


        [Test]
        public void BounceAtNearly90()
        {
            var v1 = new Velocity(new Dimensions(0.0, -99.0));
            var result = v1.Bounce(new Dimensions(1, 0.000001).Unit);

            AreClose(new Dimensions(0.0, -99.0), result, 0.01);
        }


        [Test]
        public void BounceAt360()
        {
            var v1 = new Velocity(new Dimensions(0.0, -99.0));
            var result = v1.Bounce(new Dimensions(0, -1).Unit);

            AreClose(new Dimensions(0.0, -99.0), result);
        }

       



    }
}

