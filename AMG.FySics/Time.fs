module Time
open AMG.FySics
let Tick(interval : double, element : Element, impulse : PendingImpulse) = 
    let newLocaiton = element.Location + element.Velocity.Vector * interval
    let newVelocityVector = element.Velocity.Vector + impulse.Impulse / element.Mass
    {element with Location = newLocaiton; Velocity = Velocity(newVelocityVector) }

