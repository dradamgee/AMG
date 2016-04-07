namespace AMG.Collections {
    public interface ITreeNode<TTree, TNode, TContent> {
        TTree GetValue();
        TNode NodeValue { get; }
        TContent Content { get; set; }
    }
}