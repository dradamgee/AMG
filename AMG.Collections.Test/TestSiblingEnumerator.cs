using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;


namespace AMG.Collections.Test {
    [TestFixture]
    public class TestSiblingEnumerator {
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BeforeEnumerate() {
            var letterNode = new LetterNode();
            var siblingEnumerator = new SiblingEnumerator<string, char, int>(letterNode);

            Assert.IsNull(siblingEnumerator.Current);
        }


        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConsturctorParameterIsNull() {
            new SiblingEnumerator<string, char, int>(null);
        }

        [Test]
        public void MoveNext() {
            var letterNode = new LetterNode();
            var siblingEnumerator = new SiblingEnumerator<string, char, int>(letterNode);

            siblingEnumerator.MoveNext();

            Assert.AreEqual(letterNode, siblingEnumerator.Current);
        }

        [Test]
        public void MoveNext_ResultIsTrue() {
            var letterNode = new LetterNode();
            var siblingEnumerator = new SiblingEnumerator<string, char, int>(letterNode);
            
            var moveNext = siblingEnumerator.MoveNext();

            Assert.IsTrue(moveNext);
        }
        
        [Test]
        public void MoveNextTwice_CurrrentIsReturned() {
            var letterNode1 = new LetterNode();
            var letterNode2 = new LetterNode();
            letterNode1.NextSibling = letterNode2;
            letterNode2.PreviousSibling = letterNode1;
            var siblingEnumerator = new SiblingEnumerator<string, char, int>(letterNode1);
            siblingEnumerator.MoveNext();
            
            siblingEnumerator.MoveNext();

            Assert.AreSame(letterNode2, siblingEnumerator.Current);
        }

        [Test]
        public void MoveNextTwice_ResultIsTrue() {
            var letterNode1 = new LetterNode();
            var letterNode2 = new LetterNode();
            letterNode1.NextSibling = letterNode2;
            letterNode2.PreviousSibling = letterNode1;
            var siblingEnumerator = new SiblingEnumerator<string, char, int>(letterNode1);
            siblingEnumerator.MoveNext();

            var moveNext = siblingEnumerator.MoveNext();

            Assert.IsTrue(moveNext);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MoveNextToEnd() {
            var letterNode1 = new LetterNode();
            var letterNode2 = new LetterNode();
            letterNode1.NextSibling = letterNode2;
            letterNode2.PreviousSibling = letterNode1;
            var siblingEnumerator = new SiblingEnumerator<string, char, int>(letterNode1);

            siblingEnumerator.MoveNext();
            siblingEnumerator.MoveNext();
            siblingEnumerator.MoveNext();

            Assert.IsNull(siblingEnumerator.Current); //throws
        }


        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Reset() {
            var letterNode = new LetterNode();
            var siblingEnumerator = new SiblingEnumerator<string, char, int>(letterNode);
            siblingEnumerator.MoveNext();

            siblingEnumerator.Reset();

            Assert.IsNull(siblingEnumerator.Current); //throws
        }
    }
}
