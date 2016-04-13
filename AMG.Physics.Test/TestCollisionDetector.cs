using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace AMG.Physics.Test
{
    [TestFixture]
    public class TestCollisionDetector
    {
        public IEnumerable<Func<IEnumerable<IElement>, ICollisionDetector>> TestCases()
        {
            yield return (e) => new StatefullCollisionDetector(e);
            yield return (e) => new PairCollisionDetector(e);
        }

        [Test]
        [TestCaseSource("TestCases")]
        public void TwoObjectsVeryClose_Detected(Func<IEnumerable<IElement>, ICollisionDetector> collisionDetectorFactory)
        {
            Mock<IElement> e1 = new Mock<IElement>();
            e1.SetupGet(e => e.Radius).Returns(10.0);
            e1.SetupGet(e => e.Location).Returns(new Dimensions(100.0, 100.0));

            Mock<IElement> e2 = new Mock<IElement>();
            e2.SetupGet(e => e.Radius).Returns(10.0);
            e2.SetupGet(e => e.Location).Returns(new Dimensions(101.0, 101.0));

            var elements = new IElement[] { e1.Object, e2.Object, };
            
            var detector = collisionDetectorFactory(elements);
            
            var collisionCandidates = detector.Detect().ToArray();
            Assert.IsNotNull(collisionCandidates);
            Assert.AreEqual(1, collisionCandidates.Length);
            Assert.AreEqual(e1.Object, collisionCandidates[0].Item1);
            Assert.AreEqual(e2.Object, collisionCandidates[0].Item2);
        }

       
        [TestCaseSource("TestCases")]
        public void TwoObjectsVeryFar_NotDetected(Func<IEnumerable<IElement>, ICollisionDetector> collisionDetectorFactory) {
            Mock<IElement> e1 = new Mock<IElement>();
            e1.SetupGet(e => e.Radius).Returns(10.0);
            e1.SetupGet(e => e.Location).Returns(new Dimensions(1000.0, 1000.0));

            Mock<IElement> e2 = new Mock<IElement>();
            e2.SetupGet(e => e.Radius).Returns(10.0);
            e2.SetupGet(e => e.Location).Returns(new Dimensions(101.0, 101.0));

            var elements = new IElement[] { e1.Object, e2.Object, };

            var detector = collisionDetectorFactory(elements);

            var collisionCandidates = detector.Detect().ToArray();
            Assert.IsNotNull(collisionCandidates);
            Assert.AreEqual(0, collisionCandidates.Length);
        }
    }
}