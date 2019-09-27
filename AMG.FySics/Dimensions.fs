namespace AMG.FySics

//module Banana = 
    type Vector(X: float, Y: float) =             
        member this.X = X
        member this.Y = Y
        member this.Magnitude = (X**2.0 + Y**2.0)**0.5
        member this.Unit = Unit(X / this.Magnitude, Y / this.Magnitude) 
        member this.Inverse = Vector(Y, X)
        static member (*) (n : float, d: Vector) = Vector (d.X * n, d.Y * n)
        static member (*) (d: Vector, n : float) = n * d
        static member (*) (d1: Vector, d2: Vector) = d1.X*d2.X + d1.Y*d2.Y
        static member (*) (d1: Vector, d2: Unit) = d1.X*d2.X + d1.Y*d2.Y
        static member (*) (d1: Unit, d2: Vector) = d1.X*d2.X + d1.Y*d2.Y
        static member (/) (d: Vector, n : float) = Vector (d.X / n, d.Y / n)
        static member (+) (d1 : Vector, d2: Vector) = Vector (d1.X + d2.X, d1.Y + d2.Y)
        static member (-) (d1 : Vector, d2: Vector) = Vector (d1.X - d2.X, d1.Y - d2.Y)
        static member (~-) (d : Vector) = d * -1.0
        override this.ToString() = this.X.ToString() + "|" + this.Y.ToString()
    and Unit(X: float, Y: float) =
        member this.X = X
        member this.Y = Y
        member this.Magnitude = 1
        static member (*) (n : float, u: Unit) = Vector (u.X * n, u.Y * n)
        static member (*) (d: Unit, n : float) = n * d

            
    type Velocity(Vector : Vector) =        
        member this.Vector = Vector
        member this.Act(location : Vector, interval: float) = 
            location + (Vector * interval)
        member this.Bounce(direction : Unit, loss: double) = 
            let impulse = (-Vector.X * direction.X - Vector.Y * direction.Y)
            let impulseVector = direction * impulse ;
            if impulse > 0.0 then (Vector + (impulseVector * 2.0))* loss else Vector
        member this.Bounce(direction : Unit) = this.Bounce (direction, 1.0)
            
        override this.ToString() = this.Vector.ToString()
    
    type IElement = 
        abstract member Id : int with get
        abstract member Location : Vector with get, set
        abstract member Velocity : Velocity with get, set
        abstract member Radius : float with get
        abstract member Mass : float with get        
        abstract member Top : float with get        
        abstract member Left : float with get        
        abstract member Bottom : float with get        
        abstract member Right : float with get              

    type PendingImpulse(Element:IElement, Impulse:Vector) = 
        member this.Element = Element
        member this.Impulse = Impulse
    

    type CollisionNew (loss : float) =   
        member this.Act(e1 : IElement, e2 : IElement) =
            if e1 = e2 then failwith "Cant collide with self"
            let sumOfRadii = e1.Radius + e2.Radius
            let distance = e1.Location - e2.Location
            let impact = (e1.Velocity.Vector * distance.Unit - e2.Velocity.Vector * distance.Unit) 
            let areDiverging = impact >= 0.0
            let hysterisys = if areDiverging then 150.0 else 150.0 * loss
            if distance.Magnitude > sumOfRadii
                then None 
                else
                    let compression = (sumOfRadii - distance.Magnitude) / sumOfRadii
                    Some(
                        distance.Unit * compression * (e1.Mass + e2.Mass) * hysterisys
                    )


    type Collision (Loss : float) =         
        member this.Act(e1 : IElement, e2 : IElement) =
            if e1 = e2 then failwith "Cant collide with self"
            let sumOfRadii = e1.Radius + e2.Radius
            let distance = e1.Location - e2.Location
            let impact = (e1.Velocity.Vector * distance.Unit - e2.Velocity.Vector * distance.Unit) 
            let areDiverging = impact >= 0.0
            if distance.Magnitude > sumOfRadii or areDiverging
                then None 
                else                     
                    Some(
                        -2.0 *
                        Loss *
                        distance.Unit *
                        impact
                        * (e1.Mass * e2.Mass) 
                        / (e1.Mass + e2.Mass)
                        ) 

    type Gravity(Acceleration : float) = 
        let Direction = new Vector(0.0, 1.0)     
        member this.Act(e : IElement, interval: float) =
            PendingImpulse(e, Direction * e.Mass * Acceleration * interval)
    
    type Drag(viscosity : float) = 
        member this.Act(e : IElement, interval: float) =
            let force = -e.Velocity.Vector * viscosity * e.Mass * interval
            PendingImpulse(e,  force)
 
    type IForce =
        abstract member Act: float -> PendingImpulse list

    type Leash (pin : Vector, e1 : IElement, length : float, modulus : float) =        
        member this.Pin = pin
        member this.E1 = e1
        interface IForce with 
            member this.Act(interval: float) =
                let distance = e1.Location - pin
                let impact = (e1.Velocity.Vector * distance.Unit) 
                let compression = (length - distance.Magnitude)  
                if compression = 0.0
                    then []
                    else 
                        let impulse = distance.Unit * compression * modulus * interval
                        [(PendingImpulse(e1, impulse))]

    type Bond (e1 : IElement, e2 : IElement, length : float, modulus : float) =        
        member this.E1 = e1
        member this.E2 = e2
        interface IForce with 
            member this.Act(interval: float) =
                if e1 = e2 then failwith "Cant bond with self"            
                let distance = e1.Location - e2.Location
                let compression = (length - distance.Magnitude)  
                if compression = 0.0
                    then []
                    else 
                        let impulse = distance.Unit * compression * modulus * interval
                        [PendingImpulse(e1, impulse); PendingImpulse(e2, -impulse)]
                    

                    
