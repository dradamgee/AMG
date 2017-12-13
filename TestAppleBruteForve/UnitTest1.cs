using System;
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
            var asd = sut.Execute();
        }
    }
}
