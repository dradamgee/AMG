module TestDrag

open NUnit.Framework
open AMG.FySics



[<TestFixture>]
type TestDrag() = 
    [<Test>]
    member TestDrag.WhenObjectIsSlow_DragIsLIinear_FunOf_Viscosity_Speed_Time() =
        let sut = Drag(0.07)
        let location = Vector(0.0, 0.0)
        let velocity = Velocity(Vector(0.13, 0.0))

        let element = [{Id = 0; Location = location; Velocity = velocity; Mass = 1.0; Radius = 1.0};]
        
        let pendingImpulse = Seq.head ((sut :> ITimeDependentAction).Act 0.03 element)

        Assert.AreEqual(0.000273, pendingImpulse.Impulse.Magnitude) 

    [<Test>]
    member TestDrag.WhenObjectIsSlow_DragIsLIinear_FunOf_Viscosity_Speed_Time_Radius() =
        let sut = Drag(0.07)
        let mutable location = Vector(0.0, 0.0)
        let mutable velocity = Velocity(Vector(0.13, 0.0))

        let element = [{Id = 0; Location = location; Velocity = velocity; Mass = 1.0; Radius = 0.5};]
        
        let pendingImpulse = Seq.head ((sut :> ITimeDependentAction).Act 0.03 element)

        Assert.AreEqual(0.0001365, pendingImpulse.Impulse.Magnitude) 


    [<Test>]
    member TestDrag.WithHighViscosity_DragCantReverseDirection() =
        let sut = Drag(100000.0)
        let mutable location = Vector(0.0, 0.0)
        let mutable velocity = Velocity(Vector(1.0, 0.0))

        let element = [{Id = 0; Location = location; Velocity = velocity; Mass = 1.0; Radius = 1.0};]

        let pendingImpulse = Seq.head ((sut :> ITimeDependentAction).Act 0.03 element)

        Assert.AreEqual(1.0, pendingImpulse.Impulse.Magnitude)
