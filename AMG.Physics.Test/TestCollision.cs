using AMG.FySics;
using Moq;
using NUnit.Framework;

namespace AMG.Physics.Test
{
    [TestFixture]
    public class TestCollision : TestAsserts
    {

        // time to use someone else's maths.
        //http://www.vobarian.com/collisions/2dcollisions2.pdf
        Collision Collision = new Collision(1.0);

        [Test]
        public void HeadOnCollision() {
            var e1IsAt = new Dimensions(100, 0);
            var e1IsGoing = new Velocity(new Dimensions(-10, 0));
            Mock<IElement> e1 = MockElement(e1IsAt, e1IsGoing);

            var e2IsAt = new Dimensions(90, 0);
            var e2IsGoing = new Velocity(new Dimensions(10, 0));
            Mock<IElement> e2 = MockElement(e2IsAt, e2IsGoing);

            var result = Collision.Act(e1.Object, e2.Object);

            AreClose(new Dimensions(20, 0), result.Value);
        }



        private static Mock<IElement> MockElement(Dimensions e1IsAt, Velocity e1IsGoing)
        {
            Mock<IElement> e1;
            e1 = new Mock<IElement>();
            e1.SetupGet(e => e.Radius).Returns(10.0);
            e1.SetupGet(e => e.Location).Returns(e1IsAt);
            e1.SetupGet(e => e.Velocity).Returns(e1IsGoing);
            e1.SetupGet(e => e.Mass).Returns(1.0);
            return e1;
        }
    }
}