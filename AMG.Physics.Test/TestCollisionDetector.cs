using AMG.FySics;
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
        public IEnumerable<Func<ICollisionDetector>> TestCases()
        {
            //yield return (e) => new StatefullCollisionDetector(e);
            yield return () => new PairCollisionDetector();
            //yield return (e) => new QuadTreeCollisionDetector(e, new Boundry(new Vector(2000, 2000), e, 1.0));
        }

        [Test]
        [TestCaseSource("TestCases")]
        public void TwoObjectsVeryClose_Detected(Func<IEnumerable<Element>, ICollisionDetector> collisionDetectorFactory)
        {
            Mock<Element> e1 = new Mock<Element>();
            e1.SetupGet(e => e.Id).Returns(1);
            e1.SetupGet(e => e.Radius).Returns(10.0);
            e1.SetupGet(e => e.Location).Returns(new Vector(100.0, 100.0));

            Mock<Element> e2 = new Mock<Element>();
            e2.SetupGet(e => e.Id).Returns(2);
            e2.SetupGet(e => e.Radius).Returns(10.0);
            e2.SetupGet(e => e.Location).Returns(new Vector(101.0, 101.0));

            var elements = new Element[] { e1.Object, e2.Object, };
            
            var detector = collisionDetectorFactory(elements);
            
            var collisionCandidates = detector.Detect(elements).ToArray();
            Assert.IsNotNull(collisionCandidates);
            Assert.AreEqual(1, collisionCandidates.Length);
            Assert.AreEqual(e1.Object, collisionCandidates[0].Item1);
            Assert.AreEqual(e2.Object, collisionCandidates[0].Item2);
        }

       
        [TestCaseSource("TestCases")]
        public void TwoObjectsVeryFar_NotDetected(Func<IEnumerable<Element>, ICollisionDetector> collisionDetectorFactory) {
            Mock<Element> e1 = new Mock<Element>();
            e1.SetupGet(e => e.Id).Returns(1);
            e1.SetupGet(e => e.Radius).Returns(10.0);
            e1.SetupGet(e => e.Location).Returns(new Vector(950.0, 950.0));

            Mock<Element> e2 = new Mock<Element>();
            e2.SetupGet(e => e.Id).Returns(2);
            e2.SetupGet(e => e.Radius).Returns(10.0);
            e2.SetupGet(e => e.Location).Returns(new Vector(101.0, 101.0));

            var elements = new Element[] { e1.Object, e2.Object, };

            var detector = collisionDetectorFactory(elements);

            var collisionCandidates = detector.Detect(elements).ToArray();
            Assert.IsNotNull(collisionCandidates);
            Assert.AreEqual(0, collisionCandidates.Length);
        }
    }
}