using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AMG.DocumentScanner.Test {
    [TestClass]
    public class TestDocumentScanner {
        private static Stream CreateStream(string inputText) {
            Stream inputStream = new MemoryStream();
            var streamWriter = new StreamWriter(inputStream);
            streamWriter.Write(inputText);
            streamWriter.Flush();
            inputStream.Position = 0;
            return inputStream;
        }

        [TestMethod]
        public void DocumentHasOneWord_Scan_WordIsReturnedInOutput() {
            var inputStream = CreateStream("Hello");

            var documentScanner = new StreamTokenizer();
            
            IEnumerable<string> output = documentScanner.Tokenize(inputStream);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Contains("Hello"));
        }


        [TestMethod]
        public void DocumentHasTwosWord_Scan_WordIsReturnedInOutput() {
            var inputStream = CreateStream("Hello World");
            
            var documentScanner = new StreamTokenizer();
            IEnumerable<string> output = documentScanner.Tokenize(inputStream);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Contains("Hello"));
            Assert.IsTrue(output.Contains("World"));
        }
        
        [TestMethod]
        public void DocumentHasTwosWordWithPunctuation_Scan_WordIsReturnedInOutputWithoutPunctuation() {
            var inputStream = CreateStream("Hello World!");
            
            var documentScanner = new StreamTokenizer();
            IEnumerable<string> output = documentScanner.Tokenize(inputStream);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Contains("Hello"));
            Assert.IsTrue(output.Contains("World"));
        }

        [TestMethod]
        public void DoDo_Scan_DoDoreturned() {
            var inputStream = CreateStream("Go do that voodoo that you do so well");

            var documentScanner = new StreamTokenizer();
            IEnumerable<string> output = documentScanner.Tokenize(inputStream);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Contains("Go"));
            Assert.IsTrue(output.Contains("do"));
            Assert.IsTrue(output.Contains("that"));
            Assert.IsTrue(output.Contains("voodoo"));
            Assert.IsTrue(output.Contains("you"));
            Assert.IsTrue(output.Contains("so"));
            Assert.IsTrue(output.Contains("well"));
        }


        [TestMethod]
        public void DocumentStartsWithDelimiter_Scan_ResultOnlyContainsWords() {
            var inputStream = CreateStream(" Hello World!");

            var documentScanner = new StreamTokenizer();
            IEnumerable<string> output = documentScanner.Tokenize(inputStream);
            
            Assert.IsNotNull(output);
            Assert.IsTrue(output.Contains("Hello"));
            Assert.IsTrue(output.Contains("World"));
            Assert.IsFalse(output.Contains(string.Empty));
        }
    }
}

