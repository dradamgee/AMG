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
        static member (*) (d1: Dimensions, d2: Dimensions) = d1.X*d2.X + d1.Y*d2.Y
        static member (*) (d1: Dimensions, d2: Unit) = d1.X*d2.X + d1.Y*d2.Y
        static member (*) (d1: Unit, d2: Dimensions) = d1.X*d2.X + d1.Y*d2.Y
        static member (/) (d: Dimensions, n : float) = Dimensions (d.X / n, d.Y / n)
        static member (+) (d1 : Dimensions, d2: Dimensions) = Dimensions (d1.X + d2.X, d1.Y + d2.Y)
        static member (-) (d1 : Dimensions, d2: Dimensions) = Dimensions (d1.X - d2.X, d1.Y - d2.Y)
        static member (~-) (d : Dimensions) = d * -1.0
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
        member this.Bounce(dimensions : Unit) = 
            let impulse = (-Dimensions.X * dimensions.X - Dimensions.Y * dimensions.Y)
            let impulseVector = dimensions * impulse;
            if impulse > 0.0 then Dimensions + (impulseVector * 2.0) else Dimensions        

    type IElement = 
        abstract member Location : Dimensions with get, set
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
                else                     
                    Some(
                        -2.0 *
                        Loss *
                        distance.Unit *
                        (e1.Velocity.Dimensions * distance.Unit - e2.Velocity.Dimensions * distance.Unit) 
                        * (e1.Mass * e2.Mass) 
                        / (e1.Mass + e2.Mass)
                        ) 

    type Gravity(Acceleration : float) = 
        let Direction = new Dimensions(0.0, 1.0)     
        member this.Act(e : IElement, interval: float) =
            Direction * e.Mass * Acceleration * interval
