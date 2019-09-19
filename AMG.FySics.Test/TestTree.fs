module TestTree

open NUnit.Framework
open Tree

[<TestFixture>]
type TestTree() =
    let path1 = "1.fs"
    let path2 = "2.fs"
    let path3 = "3.fs"
    let path4 = "4.fs"
     
    [<Test>]
    member TestTree.TestSingleNode() =
        let rootNode = TreeNode("Root", "Root", [[path1]])
        Assert.AreEqual(1, rootNode.Count)
        

