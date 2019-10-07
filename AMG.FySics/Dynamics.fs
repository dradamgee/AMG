namespace AMG.FySics

    
    type PendingImpulse(Element:IElement, Impulse:Vector) = 
        member this.Element = Element
        member this.Impulse = Impulse
    
    //type ITimeDependentAction =
    //    abstract member Act: float -> PendingImpulse list
   
    type Collision (loss : float) =   
        member this.Act(e1 : IElement, e2 : IElement) =
            if e1 = e2 then failwith "Cant collide with self"
            let sumOfRadii = e1.Radius + e2.Radius
            let distance = e1.Location - e2.Location
            let impact = (e1.Velocity.Vector * distance.Unit - e2.Velocity.Vector * distance.Unit) 
            let areDiverging = impact >= 0.0
            let hysterisys = if areDiverging then 1000.0 else 1000.0 * loss
            if distance.Magnitude > sumOfRadii
                then None 
                else
                    let compression = (sumOfRadii - distance.Magnitude) / sumOfRadii
                    Some(
                        distance.Unit * compression * (e1.Mass + e2.Mass) * hysterisys
                    )


    type CollisionOld (Loss : float) =         
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
            let impulse = -e.Velocity.Vector * viscosity * e.Radius * interval
            if impulse.Magnitude > e.Velocity.Vector.Magnitude * e.Mass 
                then PendingImpulse(e, -e.Velocity.Vector * e.Mass)
                else PendingImpulse(e, impulse)
 
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
                    

                    
