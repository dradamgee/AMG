using System.Linq;
using Moq;
using NUnit.Framework;

namespace AMG.Physics.Test
{
    [TestFixture]
    public class TestCollisionDetector
    {
        [Test]
        public void TwoObjectsVeryClose_Detected()
        {
            Mock<IElement> e1 = new Mock<IElement>();
            e1.SetupGet(e => e.Radius).Returns(10.0);
            e1.SetupGet(e => e.Location).Returns(new Dimensions(100.0, 100.0));

            Mock<IElement> e2 = new Mock<IElement>();
            e2.SetupGet(e => e.Radius).Returns(10.0);
            e2.SetupGet(e => e.Location).Returns(new Dimensions(101.0, 101.0));

            var elements = new IElement[] { e1.Object, e2.Object, };

            StatefullCollisionDetector detector = new StatefullCollisionDetector(elements);
            
//            PairCollisionDetector detector = new PairCollisionDetector(elements);

            var collisionCandidates = detector.Detect().ToArray();
            Assert.IsNotNull(collisionCandidates);
            Assert.AreEqual(1, collisionCandidates.Length);
            Assert.AreEqual(e1.Object, collisionCandidates[0].Item1);
            Assert.AreEqual(e2.Object, collisionCandidates[0].Item2);



        }
    }
}