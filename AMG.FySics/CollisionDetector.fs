module CollisionDetector

open AMG.FySics

let overlap (e1: Element, e2: Element) = 
    let sumOfRadii = e1.Radius + e2.Radius
    let distance = (e1.Location - e2.Location).Magnitude
    sumOfRadii > distance

let detect2 (e1: Element, elements: Element list) = 
    elements |> 
    List.filter (fun e2 -> overlap (e1, e2)) |>
    List.map(fun e2 -> (e1, e2) )
            
let rec detect(elements : Element list) = 
    match (elements.Head, elements.Tail) with
    | el, [] -> []
    | el, els -> List.append (detect2(elements.Head, elements.Tail)) (detect(elements.Tail))
       


