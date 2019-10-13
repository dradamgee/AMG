// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

//#load "Library1.fs"
//open AMG.FySics.Tree

let foo = 1
let bar = 3

let asd = 
    if foo > 1 
    then foo 
    else if bar > 1 then bar
    else 0


let file7 = ["OM"; "Client"; "Common"; "7.fs";]
let file0 = ["OM";]
let testfiles = [file7; file7; file0; ]
    
type TreeNode =
    | Node of string * string * List<TreeNode> * int
    | Leaf of string * string * int

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

let rec build(handle : string, fullPath : string, paths : string list list, count : int) =     
    let subPaths = groupAndCount paths
    match (handle, subPaths, count) with 
        | (handle, [], count) -> TreeNode.Leaf(handle, fullPath + "/" + handle, count)
        | (handle, subPaths, count) ->             
            let foo = subPaths |> List.map (fun (handle, subPaths, count) -> build(handle, fullPath + "/" + handle, subPaths, count))
            TreeNode.Node(handle, fullPath + "/" + handle, foo, count)


build ("", "", testfiles, 0)

// Define your library scripting code here

