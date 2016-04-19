using AMG.FySics;
using System;
using NUnit.Framework;

namespace AMG.Physics.Test
{
    public class TestAsserts
    {
        public static void AreClose(double expected, double actual, double spread = 0.000000001) {
            Assert.IsTrue(Math.Abs(expected - actual) < spread, actual + " is not very equal to " + expected);
        }

        public static void AreClose(Dimensions expected, Dimensions actual, double spread = 0.000000001) {
            AreClose(expected.X, actual.X, spread);
            AreClose(expected.Y, actual.Y, spread);
        }
    }
}