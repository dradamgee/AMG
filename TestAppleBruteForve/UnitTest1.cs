using System;
using System.Runtime.CompilerServices;
using AppleBruteForce;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestAppleBruteForce
{
    [TestClass]
    public class TestEngine
    {
        [TestMethod]
        public void TestExecute()
        {
            Engine sut = new Engine();
            var driver = sut.CreateDriver();
            sut.SetupDriver(driver);
            sut.TryThis(driver, "07979797979");
        }
    }
}
