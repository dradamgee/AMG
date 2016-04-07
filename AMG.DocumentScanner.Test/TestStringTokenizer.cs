using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AMG.DocumentScanner.Test {
    [TestClass]
    public class TestStringTokenizer {
        [TestMethod]
        public void TestMethod1() {
            StringTokenizer stringTokenizer = new StringTokenizer();

            var output = stringTokenizer.Tokenize(new string[]{"AAA"});

            Assert.AreEqual("AAA", output.First());
        }
    }
}
