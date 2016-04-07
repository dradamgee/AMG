using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMG.Collections {
    public class SiblingEnumerator<TTree, TNode, TContent> : IEnumerator<TreeNode<TTree, TNode, TContent>> {
        public SiblingEnumerator(TreeNode<TTree, TNode, TContent> firstNode) {
            if (firstNode == null)
                throw new ArgumentNullException("firstNode");
            FirstNode = firstNode;
        }

        protected TreeNode<TTree, TNode, TContent> FirstNode { get; private set; }
        
        public void Dispose() {}

        public bool MoveNext() {
            m_current = m_current == null ? FirstNode : m_current.NextSibling;
            return m_current != null;
        }

        public void Reset() {
            m_current = null;
        }

        private TreeNode<TTree, TNode, TContent> m_current;
        
        public TreeNode<TTree, TNode, TContent> Current { 
            get {
                if (m_current == null)
                    throw new InvalidOperationException();
                return m_current;
            } 
        }

        object IEnumerator.Current {
            get { return Current; }
        }
    }
}

