namespace AMG.FySics

//module Banana = 
    type Dimensions(X: float, Y: float) =             
        member this.X = X
        member this.Y = Y
        member this.Magnitude = (X**2.0 + Y**2.0)**0.5
        member this.Unit = Unit(X / this.Magnitude, Y / this.Magnitude) 
        member this.Inverse = Dimensions(Y, X)
        static member (*) (n : float, d: Dimensions) = Dimensions (d.X * n, d.Y * n)
        static member (*) (d: Dimensions, n : float) = n * d
        static member (/) (n : float, d: Dimensions) = Dimensions (d.X / n, d.Y / n)
        static member (+) (n : Dimensions, d: Dimensions) = Dimensions (d.X + n.X, d.Y + n.Y)
        static member (-) (n : Dimensions, d: Dimensions) = Dimensions (d.X - n.X, d.Y - n.Y)
        static member (~-) (n : Dimensions) = n * -1.0
        override this.ToString() = this.X.ToString() + ", " + this.Y.ToString()
    and Unit(X: float, Y: float) =
        member this.X = X
        member this.Y = Y
        member this.Magnitude = 1
        static member (*) (n : float, u: Unit) = Dimensions (u.X * n, u.Y * n)
        static member (*) (d: Unit, n : float) = n * d

    type Velocity(Dimensions : Dimensions) =        
        member this.Dimensions = Dimensions
        member this.Act(location : Dimensions, interval: float) = 
            location + (Dimensions * interval)
        member this.Bounce(dimensions : Dimensions) = 
            let impulse = (-Dimensions.X * dimensions.X - Dimensions.Y * dimensions.Y)
            if impulse > 0.0 then Dimensions + (dimensions * impulse) * 2.0 else Dimensions

    type IElement = 
        abstract member Location : Dimensions
        abstract member Velocity : Velocity with get, set
        abstract member Radius : float with get
        abstract member Mass : float with get

    type Collision (Loss : float) =         
        member this.Act(e1 : IElement, e2 : IElement) =        
            if e1 = e2 then raise (System.Exception("Cant collide with self"))        
            let sumOfRadii = e1.Radius + e2.Radius
            let distance = e1.Location - e2.Location
            if distance.Magnitude > sumOfRadii 
                then None 
                else Some(distance.Unit * Loss * (e1.Velocity.Dimensions.Magnitude + e1.Velocity.Dimensions.Magnitude)) // TODO work out the impulse.    

    type Gravity(Acceleration : float) = 
        let Direction = new Dimensions(0.0, -1.0)     
        member this.Act(e : IElement, interval: float) =
            Direction * e.Mass * Acceleration * interval
