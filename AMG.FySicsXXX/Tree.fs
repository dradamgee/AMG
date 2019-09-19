module Tree

    type TreeNode(handle: string, fullPath:string, subPaths: List<List<string>>) = 
        let SubNodes = 
            subPaths |> 
            List.groupBy (fun path -> path.Head) |> 
            List.map (fun zz -> match zz with | (x,y) -> TreeNode (x, fullPath + "/" + x, y.Tail))
        member this.Handle = handle
        member this.FullPath = fullPath
        member this.Count = SubNodes.Length
        


