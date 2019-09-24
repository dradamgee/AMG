module Tree

    type TreeNodeNEw =
        | Node of string * string * List<TreeNodeNEw>
        | Leaf of string * string
    
        member this.build(handle:string, fullPath:string, subPaths: List<List<string>> ) = 
            let subnodes = subPaths |> 
                List.groupBy (fun path -> path.Head) |> 
                List.map (
                    fun subpathGroup -> match subpathGroup with 
                    | (x, []) -> TreeNodeNEw.Leaf(x, fullPath + "/" + x) 
                    | (x,y) -> this.build (x, fullPath + "/" + x, y.Tail))
            
            match subnodes with
                | [] -> TreeNodeNEw.Leaf(handle, fullPath)
                | subnodes -> TreeNodeNEw.Node (handle, fullPath, subnodes)

        //member this.Count = 
        //    match this with 
        //        | Node(_, _, x) -> List.fold (fun node -> node.Count) 0 x 
   

    
    type TreeNode(handle: string, fullPath:string, subPaths: List<List<string>>) = 
        let SubNodes = 
            subPaths |> 
            List.groupBy (fun path -> path.Head) |> 
            List.map (fun zz -> match zz with | (x,y) -> TreeNode (x, fullPath + "/" + x, y.Tail))

        ////http://www.fssnip.net/tb/title/Tree-folding
        //let rec fold (f : 'b -> TreeNode -> 'b) (m : 'b) (t : TreeNode) : 'b = 
        //    let m' = f m t
         
         
        member this.Handle = handle
        member this.FullPath = fullPath
        member this.Count() = SubNodes.Length
        


        