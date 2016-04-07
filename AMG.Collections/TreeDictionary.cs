using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AMG.Collections {


    public class TreeDictionary<TKey, TNode, TValue> : IDictionary<TKey, TValue> where TKey : IEnumerable<TNode> {
        TreeNode<TKey, TNode, TValue> m_rootNode;
        
        public TreeDictionary(TreeNode<TKey, TNode, TValue> rootNode) {
            m_rootNode = rootNode;
        }
        
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return new TreeEnumator<TKey, TNode, TValue>(m_rootNode);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item) {
            Add(item.Key, item.Value);
        }

        public void Clear() {
            m_rootNode = m_rootNode.CreateTreeNode(null, default(TNode));
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            TValue value;
            m_rootNode.TryGetContent(item.Key.GetEnumerator(), out value);
            return value.Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            foreach (var keyValuePair in this)
            {
                array[arrayIndex++] = keyValuePair;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            throw new NotImplementedException();
        }

        public int Count {
            get { 
                int i = 0;
                foreach (var item in this)
                {
                    i++;
                }
                return i;
            }
        }
        public bool IsReadOnly {
            get { throw new NotImplementedException(); }
        }
        
        public bool ContainsKey(TKey key) {
            TValue value;
            return m_rootNode.TryGetContent(key.GetEnumerator(), out value);
        }

        public void Add(TKey key, TValue value) {
            m_rootNode.SetContent(key.GetEnumerator(), value);
        }

        public bool Remove(TKey key) {
            m_rootNode.RemoveContent(key);
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value) {
            return m_rootNode.TryGetContent(key.GetEnumerator(), out value);
        }

        public TValue this[TKey key] {
            get {
                TValue returnValue;
                if (!TryGetValue(key, out returnValue))
                {
                    throw new KeyNotFoundException();
                }
                return returnValue;

            }
            set {
                Add(key, value);
            }
        }

        public ICollection<TKey> Keys { get; private set; }
        public ICollection<TValue> Values { get; private set; }
    }
}
