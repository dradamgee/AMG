module ElementModule
open AMG.FySics

let top(element: Element) = 
    element.Location.Y + element.Radius
let right(element: Element) = 
    element.Location.X + element.Radius
let bottom(element: Element) = 
    element.Location.Y - element.Radius
let left(element: Element) = 
    element.Location.X - element.Radius

