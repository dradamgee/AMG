﻿using AMG.Physics;
using Moq;
using NUnit.Framework;

//using static NUnit.Framework.Assert;
//using static System.Math;


namespace AMG.Physics.Test
{
    [TestFixture]
    public class TestCollision : TestAsserts
    {
        [Test]
        public void HeadOnCollision() {
            Mock<IElement> e1 = new Mock<IElement>();
            var e1IsAt = new Dimensions(100, 0);
            var e1IsGoing = new Velocity(e1.Object);

            e1.SetupGet(e => e.Radius).Returns(10.0);
            e1.SetupGet(e => e.Location).Returns(e1IsAt);
            e1.SetupGet(e => e.Velocity).Returns(e1IsGoing);

            Mock<IElement> e2 = new Mock<IElement>();
            var e2IsAt = new Dimensions(90, 0);
            var e2IsGoing = new Velocity(e1.Object);

            e2.SetupGet(e => e.Radius).Returns(10.0);
            e1.SetupGet(e => e.Location).Returns(e2IsAt);
            e1.SetupGet(e => e.Velocity).Returns(e2IsGoing);

            Collision.Act(e1.Object, e2.Object);
        }
    }

    [TestFixture]
    public class TestVelocity : TestAsserts
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

       



    }
}

