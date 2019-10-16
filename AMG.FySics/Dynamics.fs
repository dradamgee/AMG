namespace AMG.FySics

    
    type PendingImpulse(Element:Element, Impulse:Vector) = 
        member this.Element = Element
        member this.Impulse = Impulse
    
    type ITimeDependentAction =
        abstract member Act: float -> Element seq -> PendingImpulse seq

    type ITimeIndependentAction =
        abstract member Act: subjects : Element seq -> PendingImpulse seq

           
    type Boundry (size : Vector, loss : double) = 
        
        let d = 1.0
        let XUp = new Unit(d, 0.0);
        let XDown = new Unit(-d, 0.0);
        let YUp = new Unit(0.0, d);
        let YDown = new Unit(0.0, -d);

        member this.Size = size

        member this.bounce(element: Element, direction : Unit) = 
            let velocity = element.Velocity.Vector
            let impulse = (-velocity.X * direction.X - velocity.Y * direction.Y) 
                            * element.Mass 
                            * 2.0
                            * loss
            let impulseVector = direction * impulse ;
            if impulse > 0.0 then Some(PendingImpulse(element, impulseVector)) else None


        member this.Bounce(element: Element) = 
            if ElementModule.right element > size.X && element.Velocity.Vector.X > 0.0
            then this.bounce(element, XDown) 
            else if ElementModule.left element < 0.0 && element.Velocity.Vector.X < 0.0
            then this.bounce(element, XUp)
            else if ElementModule.top element > size.Y && element.Velocity.Vector.Y > 0.0
            then this.bounce(element, YDown)
            else if ElementModule.bottom element < 0.0 && element.Velocity.Vector.Y < 0.0
            then this.bounce(element, YUp)
            else None
        
        interface ITimeDependentAction with 
            member this.Act interval elements = 
                elements |> Seq.choose this.Bounce
   
    type SoftSphereCollision (loss : float) =   
        member this.Act(e1 : Element, e2 : Element) =
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
                    
    type HardSphereCollision (Loss : float) =         
        member this.Act(e1 : Element, e2 : Element) =
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
    
    type Collision (loss : float) =         
        let collision = SoftSphereCollision(loss)
        member this.Act(e1 : Element, e2 : Element) = collision.Act(e1, e2)
            
    type Gravity(Acceleration : float) = 
        let Direction = new Vector(0.0, 1.0)     
        member this.act(interval: float, e : Element) =
            PendingImpulse(e, Direction * e.Mass * Acceleration * interval)
        interface ITimeDependentAction with 
            member this.Act interval elements = 
                elements |> Seq.map(fun e -> this.act(interval, e))

    type Drag(viscosity : float) = 
        member this.act(interval: float, e : Element) =
            let impulse = -e.Velocity.Vector * viscosity * e.Radius * interval
            if impulse.Magnitude > e.Velocity.Vector.Magnitude * e.Mass 
                then PendingImpulse(e, -e.Velocity.Vector * e.Mass)
                else PendingImpulse(e, impulse)
        interface ITimeDependentAction with 
            member this.Act interval elements = 
                elements |> Seq.map(fun e -> this.act(interval, e))

     type Leash (pin : Vector, length : float, modulus : float) =        
        member this.Pin = pin        
            member this.Act(interval: float, e1 : Element) =                
                let distance = e1.Location - pin
                let impact = (e1.Velocity.Vector * distance.Unit) 
                let compression = (length - distance.Magnitude)  
                if compression = 0.0
                    then []
                    else 
                        let impulse = distance.Unit * compression * modulus * interval
                        [(PendingImpulse(e1, impulse))]


    type Bond (length : float, modulus : float) =        
        //interface IForce with 
            member this.Act(interval: float, e1 : Element, e2 : Element) =
                if e1 = e2 then failwith "Cant bond with self"            
                let distance = e1.Location - e2.Location
                let compression = (length - distance.Magnitude)  
                if compression = 0.0
                    then []
                    else 
                        let impulse = distance.Unit * compression * modulus * interval
                        [PendingImpulse(e1, impulse); PendingImpulse(e2, -impulse)]
                    

                    
