namespace AMG.FySics

type Unit(X: double, Y: double) =
    member this.X = X
    member this.Y = Y
    member this.Magnitude = 1

type Dimensions(X: double, Y: double) =             
    member this.X = X
    member this.Y = Y
    member this.Magnitude = (X**2.0 + Y**2.0)**0.5
    member this.Unit = Unit(X / this.Magnitude, Y / this.Magnitude) 
    member this.Inverse = Dimensions(Y, X)
    static member (*) (n : double, d: Dimensions) = Dimensions (d.X * n, d.Y * n)
    static member (*) (d: Dimensions, n : double) = n * d
    static member (/) (n : double, d: Dimensions) = Dimensions (d.X / n, d.Y / n)
    static member (+) (n : Dimensions, d: Dimensions) = Dimensions (d.X + n.X, d.Y + n.Y)
    static member (-) (n : Dimensions, d: Dimensions) = Dimensions (d.X - n.X, d.Y - n.Y)
    static member (~-) (n : Dimensions) = n * -1.0
    override this.ToString() = this.X.ToString() + ", " + this.Y.ToString()
    
type Velocity(Element : IElement) =
    member this.Element = Element
    member this.Dimensions = Dimensions(0.0, 0.0)
    member this.Act(interval: double) = 
        this.Element.Location = this.Element.Location + (this.Dimensions * interval)
    member this.Bounce(dimensions : Dimensions) = 
        let impulse = (-this.Dimensions.X * dimensions.X - this.Dimensions.Y * dimensions.Y)
        if impulse > 0.0 then this.Dimensions + (dimensions * impulse) * 2.0 else this.Dimensions

and IElement = 
    abstract member Location : Dimensions
    abstract member Velocity : Velocity
    abstract member Radius : double with get
//
//type Collision = 
//    static member Act (e1 : Element, e2 Element) =
//        let _distance = e1.Radius + e2.Radius
//        if e1 == e2 throw 