using System;
using System.Collections;
using System.Collections.Generic;

namespace AMG.Collections {
    public abstract class TreeNode<TTree, TNode, TContent> {
        private readonly Dictionary<TNode, TreeNode<TTree, TNode, TContent>> m_childNodes = new Dictionary<TNode, TreeNode<TTree, TNode, TContent>>();
        protected TreeNode<TTree, TNode, TContent> ParentNode;

        public TreeNode(TreeNode<TTree, TNode, TContent> parentNode, TNode character) {
            NodeValue = character;
            ParentNode = parentNode;
        }

        public TNode NodeValue { get; private set; }
        public TContent Content { get; set; }
        public bool HasContent { get; set; }

        public TreeNode<TTree, TNode, TContent> FirstChild { get; set; }
        public TreeNode<TTree, TNode, TContent> LastChild { get; set; }
        public TreeNode<TTree, TNode, TContent> PreviousSibling { get; set; }
        public TreeNode<TTree, TNode, TContent> NextSibling { get; set; }

        public abstract TTree GetValue();

        public void SetContent(IEnumerable<TNode> nodes, TContent content) {
            SetContent(nodes.GetEnumerator(), content);
        }

        public void SetContent(IEnumerator<TNode> nodes, TContent content) {
            UpdateContent(nodes, i => content);
        }

        public void UpdateContent(IEnumerator<TNode> nodes, Func<TContent, TContent> function) {
            Perform(nodes, node => UpdateContent(node, function));
        }

        private bool UpdateContent(TreeNode<TTree, TNode, TContent> node, Func<TContent, TContent> function) {
            node.Content = function(node.Content);
            node.HasContent = true;
            return true;
        }

        protected abstract TreeNode<TTree, TNode, TContent> CreateNode(TNode nodeValue);

        public bool TryGetContent(IEnumerator<TNode> nodes, out TContent content) {
            TreeNode<TTree, TNode, TContent> node;
            if (TryGetNode(nodes, out node) && node.HasContent)
            {
                content = node.Content;
                return true;
            }
            else
            {
                content = default(TContent);
                return false;
            }
        }

        public bool TryGetNode(IEnumerator<TNode> nodes, out TreeNode<TTree, TNode, TContent> node) {
            TreeNode<TTree, TNode, TContent> nextNode;
            if (nodes.MoveNext())
            {
                if (m_childNodes.TryGetValue(nodes.Current, out nextNode))
                {
                    return nextNode.TryGetNode(nodes, out node);
                }
                node = null;
                return false;
            }
            node = this;
            return true;
        }


        public TFunction Perform<TFunction>(IEnumerator<TNode> nodes, Func<TreeNode<TTree, TNode, TContent>, TFunction> function) {
            if (nodes.MoveNext())
            {
                TreeNode<TTree, TNode, TContent> nextNode;
                if (!m_childNodes.TryGetValue(nodes.Current, out nextNode))
                {
                    nextNode = CreateNode(nodes.Current);

                    nextNode.PreviousSibling = LastChild;
                    if (LastChild != null)
                    {
                        LastChild.NextSibling = nextNode;
                    }

                    m_childNodes.Add(nextNode.NodeValue, nextNode);
                    if (FirstChild == null)
                        FirstChild = nextNode;
                    LastChild = nextNode;
                }
                return nextNode.Perform(nodes, function);
            }
            else
            {
                return function(this);
            }
        }

        public abstract TreeNode<TTree, TNode, TContent> CreateTreeNode(TreeNode<TTree, TNode, TContent> parentNode, TNode nextNodeValue);

        public bool MoveToNext(out TreeNode<TTree, TNode, TContent> nextNode) {
            if (FirstChild != null)
                return FirstChild.MoveTo(out nextNode);
            if (NextSibling != null)
                return NextSibling.MoveTo(out nextNode);
            if (ParentNode != null)
                return ParentNode.MoveUp(out nextNode);
            nextNode = null;
            return false;
        }

        public bool MoveUp(out TreeNode<TTree, TNode, TContent> nextNode) {
            if (NextSibling != null)
                return NextSibling.MoveTo(out nextNode);
            if (ParentNode != null)
                return ParentNode.MoveUp(out nextNode);
            nextNode = null;
            return false;
        }

        public bool RemoveContent(IEnumerable<TNode> nodes) {
            return Perform(nodes.GetEnumerator(), RemoveContent);
        }

        private bool RemoveContent(TreeNode<TTree, TNode, TContent> node) {
            if (!node.HasContent)
                return false;

            node.Content = default(TContent);
            node.HasContent = false;
            return true;
        }

        public bool MoveTo(out TreeNode<TTree, TNode, TContent> nextNode) {
            if (HasContent)
            {
                nextNode = this;
                return true;
            }
            else
            {
                return MoveToNext(out nextNode);
            }
        }

    }

    public class LetterNode : TreeNode<string, char, int> {
        public LetterNode() : this(null, char.MinValue) {
        }

        public LetterNode(TreeNode<string, char, int> parentNode, char character) : base(parentNode, character) {
        }

        public override string GetValue() {
            if (ParentNode == null) // if this is the root node return nothing.
                return string.Empty;
            return ParentNode.GetValue() + NodeValue;
        }

        protected override TreeNode<string, char, int> CreateNode(char nodeValue) {
            return CreateTreeNode(this, nodeValue);
        }

        public override TreeNode<string, char, int> CreateTreeNode(TreeNode<string, char, int> parentNode, char nextNodeValue) {
            return new LetterNode(parentNode, nextNodeValue);
        }
    }
}