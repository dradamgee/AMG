using AMG.FySics;
using System;
using NUnit.Framework;

namespace AMG.Physics.Test
{
    [TestFixture]
    public class TestDimensions : TestAsserts
    {
        [Test]
        public void TestUnit() {
            var unit = new Dimensions(1.0, 1.0).Unit;
            AreClose(Math.Pow(0.5, 0.5), unit.X);
            AreClose(Math.Pow(0.5, 0.5), unit.Y);
        }

        [Test]
        public void TestDotProduct()
        {
            var unitAt45 = new Dimensions(1.0, 1.0).Unit * 1.0;
            var horitontalVector = new Dimensions(100.0, 0.0);

            var result = horitontalVector * unitAt45;

            AreClose(70.710678118654741, result);
        }
    }
}