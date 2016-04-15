namespace AMG.FySics

type Dimensions(X: double, Y: double) = 
    member this.X = X
    member this.Y = Y
    member this.Magnitude = (X**2.0 + Y**2.0)**0.5
    member this.Unit = new Dimensions(1.0, 1.0) /// this.Magnitude
    

//type Dimensions = {
//    X: double
//    Y: double
//}  

module DimensionMaths = 
        
    let Multiply (d1:Dimensions, d2:Dimensions) =
        let x = d1.X * d2.X
        let y = d1.Y * d2.Y
        new Dimensions(x, y)
        

