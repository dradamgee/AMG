module TestLeash

open NUnit.Framework
open AMG.FySics

[<TestFixture>]
type TestLeash() = 
    [<Test>]
    member this.WhenNotStreched_NoImpulseIsReturned() =
 
        let leash = Leash(Vector(0.0, 0.0), 5.0, 1.0) 

        let location = Vector(3.0, 4.0)
        let velocity = Velocity(Vector(0.0, 0.0))
        let element = {Id = 1; Location = location; Velocity = velocity; Mass = 5.0; Radius = 1.0}
        
        let pendingImpulses = leash.Act(1.0, element)
                
        Assert.AreEqual([], pendingImpulses) 
