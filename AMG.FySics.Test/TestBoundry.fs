namespace TestBoundry
open NUnit.Framework
open AMG.FySics



[<TestFixture>]
type TestBoundry()=
    [<Test>] member TestBoundry.ElementTooFarRight_Bounce_Left() = 
        let size = Vector(20.0, 10.0)
        
        let sut = Boundry(size, 1.0)

        let location = Vector(21.0, 5.0)
        let velocity = Velocity(Vector(3.0, 0.0))
        let element = {Id = 1; Location = location; Velocity = velocity; Mass = 5.0; Radius = 1.0}

        let impulse = sut.Bounce(element)

        let impulse = match impulse with
            | Some(x) -> x.Impulse

        // impulse should reverse velocity*mass 
        //= 3.0(velocity)*5.0(mass)*-2.0(once to stop and once to reverse)
        Assert.AreEqual(-30.0, impulse.X)
    
    [<Test>] member TestBoundry.ElementTooFarLeft_Bounce_Right() = 
        let size = Vector(20.0, 10.0)
        
        let sut = Boundry(size, 1.0)

        let location = Vector(-1.0, 5.0)
        let velocity = Velocity(Vector(-3.0, 0.0))
        let element = {Id = 1; Location = location; Velocity = velocity; Mass = 5.0; Radius = 1.0}

        let impulse = sut.Bounce(element)

        let impulse = match impulse with
            | Some(x) -> x.Impulse

        Assert.AreEqual(30.0, impulse.X)

    [<Test>] member TestBoundry.ElementTooFarUp_Bounce_Down() = 
        let size = Vector(20.0, 10.0)
        
        let sut = Boundry(size, 1.0)

        let location = Vector(5.0, 11.0)
        let velocity = Velocity(Vector(0.0, 3.0))
        let element = {Id = 1; Location = location; Velocity = velocity; Mass = 5.0; Radius = 1.0}

        let impulse = sut.Bounce(element)

        let impulse = match impulse with
            | Some(x) -> x.Impulse

        Assert.AreEqual(-30.0, impulse.Y)

    [<Test>] member TestBoundry.ElementTooFarDown_Bounce_Up() = 
        let size = Vector(20.0, 10.0)
        
        let sut = Boundry(size, 1.0)

        let location = Vector(5.0, -1.0)
        let velocity = Velocity(Vector(0.0, -3.0))
        let element = {Id = 1; Location = location; Velocity = velocity; Mass = 5.0; Radius = 1.0}

        let impulse = sut.Bounce(element)

        let impulse = match impulse with
            | Some(x) -> x.Impulse

        Assert.AreEqual(30.0, impulse.Y)

