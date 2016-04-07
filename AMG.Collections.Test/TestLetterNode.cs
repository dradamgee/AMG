using System;
using NUnit.Framework;

namespace AMG.Collections.Test {
    [TestFixture]
    public class TestLetterNode {
        [Test]
        public void AddTwodifferentCharacters_FirstIsRecorded() {
            var rootNode = new LetterNode(null, char.MinValue);

            rootNode.SetContent("a".GetEnumerator(), 123);
            rootNode.SetContent("b".GetEnumerator(), 456);

            int content;
            rootNode.TryGetContent("a".GetEnumerator(), out content);
            Assert.AreEqual(123, content);
        }


        [Test]
        public void AddTwodifferentCharacters_SecondIsRecorded() {
            var rootNode = new LetterNode(null, char.MinValue);

            rootNode.SetContent("a".GetEnumerator(), 123);
            rootNode.SetContent("b".GetEnumerator(), 456);

            int content;
            rootNode.TryGetContent("b".GetEnumerator(), out content);
            Assert.AreEqual(456, content);
        }

        [Test]
        public void CreateSingleLetterWithEmptyParent() {
            var rootNode = new LetterNode(null, char.MinValue);

            var letterNode = new LetterNode(rootNode, 'c');
            Assert.AreEqual("c", letterNode.GetValue());
        }

        [Test]
        public void MoveToFirst() {
            var rootNode = new LetterNode(null, char.MinValue);

            rootNode.SetContent("cab", 1);

            TreeNode<string, char, int> nextNode;
            rootNode.MoveToNext(out nextNode);

            Assert.AreEqual('b', nextNode.NodeValue);
        }

        [Test]
        public void PerformActionOnCharacterThatExists() {
            var rootNode = new LetterNode(null, char.MinValue);

            Func<int, int> function = i => (i == 123) ? 321 : 0;
            rootNode.SetContent("a".GetEnumerator(), 123);

            rootNode.UpdateContent("a".GetEnumerator(), function);

            int content;
            rootNode.TryGetContent("a".GetEnumerator(), out content);
            Assert.AreEqual(321, content);
        }

        [Test]
        public void PerformActionOnMissingCharacter() {
            var rootNode = new LetterNode(null, char.MinValue);

            Func<int, int> function = i => 321;

            rootNode.UpdateContent("a".GetEnumerator(), function);

            int content;
            rootNode.TryGetContent("a".GetEnumerator(), out content);
            Assert.AreEqual(321, content);
        }


        [Test]
        public void RecurseToChild() {
            var rootNode = new LetterNode(null, char.MinValue);

            rootNode.SetContent("cat", 1);
            rootNode.SetContent("catch", 1);

            TreeNode<string, char, int> nextNode;
            rootNode.MoveToNext(out nextNode);
            nextNode.MoveToNext(out nextNode);

            Assert.AreEqual('h', nextNode.NodeValue);
        }

        [Test]
        public void RecurseToParent() {
            var rootNode = new LetterNode(null, char.MinValue);

            rootNode.SetContent("catch", 1);
            rootNode.SetContent("caz", 1);

            TreeNode<string, char, int> nextNode;
            rootNode.MoveToNext(out nextNode);
            nextNode.MoveToNext(out nextNode);

            Assert.AreEqual('z', nextNode.NodeValue);
        }

        [Test]
        public void RecurseToRoot() {
            var rootNode = new LetterNode(null, char.MinValue);

            rootNode.SetContent("catch", 1);

            TreeNode<string, char, int> nextNode;
            rootNode.MoveToNext(out nextNode);
            bool result = nextNode.MoveToNext(out nextNode);

            Assert.IsFalse(result);
        }

        [Test]
        public void RecurseToSidling() {
            var rootNode = new LetterNode(null, char.MinValue);

            rootNode.SetContent("cab", 1);
            rootNode.SetContent("cat", 1);

            TreeNode<string, char, int> nextNode;
            rootNode.MoveToNext(out nextNode);
            nextNode.MoveToNext(out nextNode);

            Assert.AreEqual('t', nextNode.NodeValue);
        }

        [Test]
        public void RemoveContent_WhenContentDoesntExists() {
            var rootNode = new LetterNode(null, char.MinValue);

            rootNode.SetContent("catches", 1);
            bool contentRemoved = rootNode.RemoveContent("catch");

            Assert.IsFalse(contentRemoved);
        }

        [Test]
        public void RemoveContent_WhenContentExists() {
            var rootNode = new LetterNode(null, char.MinValue);

            rootNode.SetContent("catch", 1);
            bool contentRemoved = rootNode.RemoveContent("catch");

            Assert.IsTrue(contentRemoved);
        }

        [Test]
        public void SetSingleCharacter() {
            var rootNode = new LetterNode(null, char.MinValue);

            rootNode.SetContent("a".GetEnumerator(), 1);

            int content;
            bool tryGetContent = rootNode.TryGetContent("a".GetEnumerator(), out content);
            Assert.IsTrue(tryGetContent);
        }

        [Test]
        public void SetSingleCharacterContent() {
            var rootNode = new LetterNode(null, char.MinValue);

            rootNode.SetContent("a".GetEnumerator(), 666);

            int content;
            bool tryGetContent = rootNode.TryGetContent("a".GetEnumerator(), out content);
            Assert.IsTrue(tryGetContent);
        }

        [Test]
        public void SetSingleCharacterContentTwice() {
            var rootNode = new LetterNode(null, char.MinValue);

            rootNode.SetContent("a".GetEnumerator(), 777);
            rootNode.SetContent("a".GetEnumerator(), 666);

            int content;
            rootNode.TryGetContent("a".GetEnumerator(), out content);
            Assert.AreEqual(666, content);
        }

        [Test]
        public void SetSingleCharacterTwice() {
            var rootNode = new LetterNode(null, char.MinValue);
            rootNode.SetContent("a".GetEnumerator(), 777);
            rootNode.SetContent("a".GetEnumerator(), 666);
            int content;

            bool tryGetContent = rootNode.TryGetContent("a".GetEnumerator(), out content);

            Assert.IsTrue(tryGetContent);
        }

        [Test]
        public void TryGetContent_KeyHasContent() {
            var rootNode = new LetterNode(null, char.MinValue);
            rootNode.SetContent("car".GetEnumerator(), 777);
            int content;

            bool tryGetContent = rootNode.TryGetContent("car".GetEnumerator(), out content);
            Assert.IsTrue(tryGetContent);
        }

        [Test]
        public void TryGetContent_KeyHasNoContent() {
            var rootNode = new LetterNode(null, char.MinValue);
            rootNode.SetContent("car".GetEnumerator(), 777);
            int content;

            bool tryGetContent = rootNode.TryGetContent("ca".GetEnumerator(), out content);
            Assert.IsFalse(tryGetContent);
        }
    }
}