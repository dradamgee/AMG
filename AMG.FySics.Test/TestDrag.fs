module TestDrag

open NUnit.Framework
open AMG.FySics



[<TestFixture>]
type TestDrag() = 
    [<Test>]
    member TestDrag.WhenObjectIsSlow_DragIsLIinear_FunOf_Viscosity_Speed_Time() =
        let sut = Drag(0.07)
        let mutable location = Vector(0.0, 0.0)
        let mutable velocity = Velocity(Vector(0.13, 0.0))

        let element = {
                new IElement with                    
                    member this.Id = 1
                    member this.Location with get() = location
                                         and set(value) = location <- value
                    member this.Velocity with get() = velocity
                                         and set(value) = velocity <- value
                    member this.Radius = 1.0
                    member this.Mass = 1.0
                    member this.Top = 1.0
                    member this.Left = 1.0
                    member this.Bottom = 1.0
                    member this.Right = 1.0
            }

        let pendingImpulse = sut.Act(element, 0.03)

        Assert.AreEqual(0.000273, pendingImpulse.Impulse.Magnitude) 

    [<Test>]
    member TestDrag.WhenObjectIsSlow_DragIsLIinear_FunOf_Viscosity_Speed_Time_Radius() =
        let sut = Drag(0.07)
        let mutable location = Vector(0.0, 0.0)
        let mutable velocity = Velocity(Vector(0.13, 0.0))

        let element = {
                new IElement with                    
                    member this.Id = 1
                    member this.Location with get() = location
                                         and set(value) = location <- value
                    member this.Velocity with get() = velocity
                                         and set(value) = velocity <- value
                    member this.Radius = 0.5
                    member this.Mass = 1.0
                    member this.Top = 1.0
                    member this.Left = 1.0
                    member this.Bottom = 1.0
                    member this.Right = 1.0

            }

        let pendingImpulse = sut.Act(element, 0.03)

        Assert.AreEqual(0.0001365, pendingImpulse.Impulse.Magnitude) 


    [<Test>]
    member TestDrag.WithHighViscosity_DragCantReverseDirection() =
        let sut = Drag(100000.0)
        let mutable location = Vector(0.0, 0.0)
        let mutable velocity = Velocity(Vector(1.0, 0.0))

        let element = {
                new IElement with                    
                    member this.Id = 1
                    member this.Location with get() = location
                                         and set(value) = location <- value
                    member this.Velocity with get() = velocity
                                         and set(value) = velocity <- value
                    member this.Radius = 1.0
                    member this.Mass = 1.0
                    member this.Top = 1.0
                    member this.Left = 1.0
                    member this.Bottom = 1.0
                    member this.Right = 1.0

            }

        let pendingImpulse = sut.Act(element, 1.0)

        Assert.AreEqual(1.0, pendingImpulse.Impulse.Magnitude)
