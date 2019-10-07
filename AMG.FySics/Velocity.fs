namespace AMG.FySics
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
