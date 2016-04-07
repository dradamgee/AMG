using NUnit.Framework;

namespace AMG.Collections.Test {
    [TestFixture]
    public class TestTreeEnumator {
        [Test]
        public void MoveNextTwice_OneItem() {
            var rootNode = new LetterNode();
            rootNode.SetContent("a", 1);

            var enumerator = new TreeEnumator<string, char, int>(rootNode);

            enumerator.MoveNext();
            bool result = enumerator.MoveNext();

            Assert.IsFalse(result);
        }

        [Test]
        public void MoveNext_OneItem_ContentIsCorrect() {
            var rootNode = new LetterNode();
            rootNode.SetContent("a", 1);

            var enumerator = new TreeEnumator<string, char, int>(rootNode);

            enumerator.MoveNext();

            Assert.AreEqual("a", enumerator.Current.Key);
            Assert.AreEqual(1, enumerator.Current.Value);
        }

        [Test]
        public void MoveNext_OneItem_ResultIsTrue() {
            var rootNode = new LetterNode();
            rootNode.SetContent("a", 1);

            var enumerator = new TreeEnumator<string, char, int>(rootNode);

            bool result = enumerator.MoveNext();

            Assert.IsTrue(result);
        }

        [Test]
        public void MoveResetMove_OneItem() {
            var rootNode = new LetterNode();
            rootNode.SetContent("a", 1);

            var enumerator = new TreeEnumator<string, char, int>(rootNode);

            enumerator.MoveNext();
            enumerator.Reset();
            bool result = enumerator.MoveNext();

            Assert.IsTrue(result);
        }
    }
}