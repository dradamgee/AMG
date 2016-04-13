using AMG.Physics;
using NUnit.Framework;
using System;
using static NUnit.Framework.Assert;
using static System.Math;


namespace AMG.Physics.Test
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void BounceAt180()
        {
            var v1 = new Velocity(null) {Dimensions = new Dimensions(0.0, -99.0)};
            v1.Bounce(new Dimensions(0, 1));

            AreClose(new Dimensions(0.0, 99.0), v1.Dimensions);
        }

        [Test]
        public void BounceAt45()
        {
            var v1 = new Velocity(null) { Dimensions = new Dimensions(0.0, -99.0) };
            v1.Bounce(new Dimensions(1, 1).Unit);

            AreClose(new Dimensions(99.0, 0.0), v1.Dimensions);          
        }

        [Test]
        public void BounceAtNearly90()
        {
            var v1 = new Velocity(null) { Dimensions = new Dimensions(0.0, -99.0) };
            v1.Bounce(new Dimensions(1, 0.000001).Unit);

            AreClose(new Dimensions(0.0, -99.0), v1.Dimensions, 0.01);
        }


        [Test]
        public void BounceAt360()
        {
            var v1 = new Velocity(null) { Dimensions = new Dimensions(0.0, -99.0)};
            v1.Bounce(new Dimensions(0, -1));

            AreClose(new Dimensions(0.0, -99.0), v1.Dimensions);
        }


        [Test]
        public void TestUnit()
        {
            var unit = new Dimensions(1, 1).Unit;
            AreClose(Pow(0.5, 0.5), unit.X);
            AreClose(Pow(0.5, 0.5), unit.Y);
        }
        
        public void AreClose(double expected, double actual, double spread = 0.000000001)
        {
            IsTrue(Abs(expected - actual) < spread, actual + " is not very equal to " + expected);
        }

        public void AreClose(Dimensions expected, Dimensions actual, double spread = 0.000000001)
        {
            AreClose(expected.X, actual.X, spread);
            AreClose(expected.Y, actual.Y, spread);
        }
       



    }
}

