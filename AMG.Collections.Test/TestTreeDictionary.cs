using System.Collections.Generic;
using NUnit.Framework;

namespace AMG.Collections.Test {
    public class StringDictionary : TreeDictionary<string, char, int> {
        public StringDictionary() : base(new LetterNode()) {
        }
    }

    [TestFixture(typeof(StringDictionary))]
    [TestFixture(typeof(Dictionary<string, int>))]
    public class TestTreeDictionary<T> where T : IDictionary<string, int>, new(){
        [Test]
        public void AddOneItem() {
            var dictionary = new T();
            dictionary.Add("cat", 0);
            Assert.IsTrue(dictionary.ContainsKey("cat"));
        
        }

        [Test]
        public void AddOneItemWithContent_TryGetValue() {
            var dictionary = new T();
            dictionary.Add("cat", 123);
            int value;
            dictionary.TryGetValue("cat", out value);
            Assert.AreEqual(123, value);
        }

        [Test]
        public void AddOneItemWithContent_ContainsKeyValuePair() {
            var dictionary = new T();
            
            dictionary.Add("cat", 123);
            
            Assert.IsTrue(dictionary.Contains(new KeyValuePair<string, int>("cat", 123)));
        }

        [Test]
        public void TestClear() {
            var dictionary = new T();
            dictionary.Add("cat", 123);
            int value;
            dictionary.Clear();
            Assert.IsFalse(dictionary.TryGetValue("cat", out value));
        }

        [Test]
        public void AddOneItemWithContent_GetEnumerator() {
            var dictionary = new T();
            dictionary.Add("cat", 123);

            var values = dictionary.GetEnumerator();
            values.MoveNext();

            Assert.AreEqual("cat", values.Current.Key);
            Assert.AreEqual(123, values.Current.Value);
        }

        [Test]
        public void CopyToArray() {
            var dictionary = new T();
            dictionary.Add("car", 123);
            
            KeyValuePair<string, int>[] keyValuePairs = new KeyValuePair<string, int>[1];
            dictionary.CopyTo(keyValuePairs, 0);

            Assert.AreEqual("car", keyValuePairs[0].Key);
            Assert.AreEqual(123, keyValuePairs[0].Value);
        }

        [Test]
        public void CopyToMiddleOfArray() {
            var dictionary = new T();
            dictionary.Add("car", 123);

            KeyValuePair<string, int>[] keyValuePairs = new KeyValuePair<string, int>[3];
            dictionary.CopyTo(keyValuePairs, 1);

            Assert.AreEqual("car", keyValuePairs[1].Key);
            Assert.AreEqual(123, keyValuePairs[1].Value);
        }

        [Test]
        public void RemoveItem() {
            var dictionary = new T();
            dictionary.Add("car", 123);

            dictionary.Remove("car");

            Assert.IsFalse(dictionary.ContainsKey("car"));
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestIndexer_Get() {
            var dictionary = new T();

            var returnValue = dictionary["car"];
        }


        [Test]
        public void TestIndexer__Get_KeyExists() {
            var dictionary = new T();

            dictionary.Add("car", 123);

            var returnValue = dictionary["car"];

            Assert.AreEqual(123, returnValue);
        }


        [Test]
        public void TestIndexer_Set() {
            var dictionary = new T();

            dictionary["car"] = 123;

            int returnValue;
            var tryGetValue = dictionary.TryGetValue("car", out returnValue);
            Assert.IsTrue(tryGetValue);
            Assert.AreEqual(123, returnValue);
        }
        
        [Test]
        public void TestCount() {
            var dictionary = new T();
            dictionary["car"] = 123;
            dictionary["cat"] = 123;
            dictionary["dog"] = 123;
            
            var actual = dictionary.Count;

            Assert.AreEqual(3, actual);
        }
        
    }
}
