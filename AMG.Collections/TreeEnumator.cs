using System.Collections;
using System.Collections.Generic;

namespace AMG.Collections {
    public class TreeEnumator<TTree, TNode, TContent> : IEnumerator<KeyValuePair<TTree, TContent>> {
        private readonly TreeNode<TTree, TNode, TContent> m_rootNode;
        private TreeNode<TTree, TNode, TContent> m_currentNode;

        public TreeEnumator(TreeNode<TTree, TNode, TContent> rootNode) {
            m_currentNode = m_rootNode = rootNode;
        }

        public void Dispose() {
        }

        public bool MoveNext() {
            return m_currentNode.MoveToNext(out m_currentNode);
        }

        public void Reset() {
            m_currentNode = m_rootNode;
        }

        public KeyValuePair<TTree, TContent> Current {
            get { return new KeyValuePair<TTree, TContent>(m_currentNode.GetValue(), m_currentNode.Content); }
        }

        object IEnumerator.Current {
            get { return Current; }
        }
    }
}