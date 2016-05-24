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
            var v1 = new Velocity(new Vector(0.0, -99.0)) ;
            var result = v1.Bounce(new Vector(0, 1).Unit);

            AreClose(new Vector(0.0, 99.0), result);
        }

        [Test]
        public void BounceAt45()
        {
            var v1 = new Velocity(new Vector(0.0, -99.0));
            var result = v1.Bounce(new Vector(1, 1).Unit);

            AreClose(new Vector(99.0, 0.0), result);          
        }


        public Vector Bounce(Vector Vector, Vector vector) {
            var impulse = (-Vector.X * vector.X - Vector.Y * vector.Y);

            if (impulse > 0.0) {
                var impulseVector = vector * impulse;
                return Vector + impulseVector * 2.0;
            }
            return null;
        }


        [Test]
        public void BounceAtNearly90()
        {
            var v1 = new Velocity(new Vector(0.0, -99.0));
            var result = v1.Bounce(new Vector(1, 0.000001).Unit);

            AreClose(new Vector(0.0, -99.0), result, 0.01);
        }


        [Test]
        public void BounceAt360()
        {
            var v1 = new Velocity(new Vector(0.0, -99.0));
            var result = v1.Bounce(new Vector(0, -1).Unit);

            AreClose(new Vector(0.0, -99.0), result);
        }

       



    }
}

