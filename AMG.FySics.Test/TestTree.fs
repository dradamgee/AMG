module TestTree

open NUnit.Framework
open Tree

[<TestFixture>]
type TestTree() =
    let file1 = ["1.fs";]
    let file2 = ["2.fs";]
    let file3 = ["3.fs";]
    let file4 = ["4.fs";]

    let file5 = ["Client"; "5.fs";]
    let file6 = ["Server"; "6.fs";]
    let file7 = ["OM"; "Client"; "Common"; "7.fs";]
    let file8 = ["OM"; "Client"; "Common"; "8.fs";]
    let file9 = ["OM"; "Server"; "Common"; "7.fs";]
    let file10 = ["OM"; "Server"; "Order"; "8.fs";]
    let file11 = ["Common"; "8.fs";]
     
    [<Test>]
    member TestTree.EmptyNode_CountIs0() =
        let rootNode = TreeNode("Root", "Root", [])
        Assert.AreEqual(0, rootNode.Count) 

    [<Test>]
    member TestTree.TestSingleNodeRoot() =
        let rootNode = TreeNode("Root", "Root", [file1])
        Assert.AreEqual(1, rootNode.Count)        

    [<Test>]
    member TestTree.FourUniqueNodesCountIs4() =
        let rootNode = TreeNode("Root", "Root", [file1; file2; file3; file4;])
        Assert.AreEqual(4, rootNode.Count)
        
    [<Test>]
    member TestTree.TwoSubNodes() =
        let rootNode = TreeNode("Root", "Root", [file5; file6;])
        Assert.AreEqual(2, rootNode.Count)

    [<Test>]
    member TestTree.TwoDeepNodes() =
        let rootNode = TreeNode("Root", "Root", [file7; file8;])
        Assert.AreEqual(2, rootNode.Count)
        


