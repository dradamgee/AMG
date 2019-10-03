module TestTree

open NUnit.Framework
open TreeModule

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
        let rootNode = buildRoot([])
        Assert.AreEqual(0, rootNode.Count) 

    [<Test>]
    member TestTree.TestSingleNodeRoot() =
        let rootNode = buildRoot([file1])
        Assert.AreEqual(1, rootNode.Count)        

    [<Test>]
    member TestTree.FourUniqueNodes_CountIs4() =
        let rootNode = buildRoot([file1; file2; file3; file4;])
        Assert.AreEqual(4, rootNode.Count)

    [<Test>]
    member TestTree.TwoIdenticleNodes_CountIs2() =
        let rootNode = buildRoot([file1; file1;])
        Assert.AreEqual(2, rootNode.Count)
        
    [<Test>]
    member TestTree.TwoSubNodes() =
        let rootNode = buildRoot([file5; file6;])
        Assert.AreEqual(2, rootNode.Count)

    [<Test>]
    member TestTree.TwoDeepNodes() =
        let rootNode = buildRoot([file7; file8;])
        Assert.AreEqual(2, rootNode.Count)


    [<Test>]
    member TestTree.SingleFileInRoot_OneSubnode() =
        let rootNode = buildRoot([file1;])
        let count = match rootNode with
            | Node(_, _, sn, _) -> sn.Length 
            
        Assert.AreEqual(1, count)

        
    [<Test>]
    member TestTree.SingleFileInRoot_FullPathIsCorrect() =
        let rootNode = buildRoot([file1;])
        let fileNode = match rootNode with
            | Node(_, _, sn, _) -> sn.Head
        let fullPath = match fileNode with 
            | Leaf(_, fullPath, _) -> fullPath
            
        Assert.AreEqual(@"/1.fs", fullPath)

    [<Test>]
    member TestTree.SingleFileInRoot_LableIsCorrect() =
        let rootNode = buildRoot([file1;])
        let fileNode = match rootNode with
            | Node(_, _, sn, _) -> sn.Head
        let lable = match fileNode with 
            | Leaf(lable, _, _) -> lable
            
        Assert.AreEqual(@"1.fs", lable)

    [<Test>]
    member TestTree.SingleFileInDeep_LableIsCorrect() =
        let rootNode = buildRoot([file7;]) //let file7 = ["OM"; "Client"; "Common"; "7.fs";]
        let level1 = match rootNode with     //root
            | Node(_, _, sn, _) -> sn.Head  //om
        let level2 = match level1 with       
            | Node(_, _, sn, _) -> sn.Head  //client
        let level3 = match level2 with       
            | Node(_, _, sn, _) -> sn.Head   //common
        let level4 = match level3 with       
            | Node(_, _, sn, _) -> sn.Head   //7.fs
        let lable = match level4 with       
            | Leaf(lable, _, _) -> lable    


        Assert.AreEqual(@"7.fs", lable) //TODO this should be /7.fs ????
    
    
    [<Test>]
    member TestTree.SingleFileInDeep_FullPathIsCorrect() =
        let rootNode = buildRoot([file7;]) //let file7 = ["OM"; "Client"; "Common"; "7.fs";]
        let level1 = match rootNode with     //root
            | Node(_, _, sn, _) -> sn.Head  //om
        let level2 = match level1 with       
            | Node(_, _, sn, _) -> sn.Head  //client
        let level3 = match level2 with       
            | Node(_, _, sn, _) -> sn.Head   //common
        let level4 = match level3 with       
            | Node(_, _, sn, _) -> sn.Head   //7.fs
        let fullPath = match level4 with       
            | Leaf(_, fullPath, _) -> fullPath


        Assert.AreEqual(@"/OM/Client/Common/7.fs", fullPath)


