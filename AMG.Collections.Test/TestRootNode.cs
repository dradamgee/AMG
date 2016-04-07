using NUnit.Framework;

namespace AMG.Collections.Test {
    [TestFixture]
    public class TestRootNode {
        [Test]
        public void CreateAddSingleCharacter() {
            var letterNode = new LetterNode(null, ' ');

            letterNode.SetContent("a".GetEnumerator(), 1);

            int content;
            bool tryGetContent = letterNode.TryGetContent("a".GetEnumerator(), out content);
            Assert.IsTrue(tryGetContent);
        }
    }
}