module TreeModule
    
        
type TreeNode =
    | Node of string * string * List<TreeNode> * int
    | Leaf of string * string * int
    member this.Count = 
        match this with 
            | Node(_, _, _, n) -> n
            | Leaf(_, _, n) -> n
    member this.Handle =
        match this with 
            | Node(x, _, _, _) -> x
            | Leaf(x, _, _) -> x
    member this.subNodes =
        match this with 
            | Node(_, _, x, _) -> x
            | Leaf(_, _, _) -> []



let addCount(handle : string, paths : string list list) =
    (handle, paths, paths.Length)

let tailsSimple(paths : string list list) = 
    paths |> 
    List.map (fun path -> path.Tail)

let tails(handle : string, paths : string list list, count : int) =     
    (handle, tailsSimple paths, count)

let removeEmptySimple(paths : string list list) = 
    paths |> 
    List.filter (fun a -> not a.IsEmpty)

let removeEmpty(handle : string, paths : string list list, count : int) =     
    (handle, removeEmptySimple paths, count)

let groupAndCount(files : string list list) = 
    files |>     
    List.groupBy (fun path -> path.Head) |>            
    List.map addCount |>
    List.map tails |>
    List.map removeEmpty

let rec buildTree(handle : string, fullPath : string, paths : string list list, count : int) =     
    let subPaths = groupAndCount paths
    match (handle, subPaths, count) with 
        | (handle, [], count) -> TreeNode.Leaf(handle, fullPath, count)
        | (handle, subPaths, count) ->             
            let foo = subPaths |> List.map (fun (handle, subPaths, count) -> buildTree(handle, fullPath + "/" + handle, subPaths, count))
            TreeNode.Node(handle, fullPath, foo, count)

let buildRoot (paths : string list list) = 
    buildTree("", "", paths, paths.Length)




