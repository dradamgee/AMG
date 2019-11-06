module TestTime
open NUnit.Framework
open AMG.FySics

[<TestFixture>]
type TestTime()=
    let centre = Vector(0.0, 0.0)
    let stationary = Velocity(Vector(0.0, 0.0))
    let noImpulse element= PendingImpulse(element, Vector(0.0, 0.0)) 
    
    let element = { Id = 1; Location = centre; Velocity = stationary; Mass = 5.0; Radius = 1.0}


    [<Test>] member TestTime.ElementIsMovingAlongX_TimeTicks_ElementMovesAlongX() = 

        let element = {element with Velocity = Velocity(Vector(3.0, 0.0))}
        
        let resultingElement = Time.Tick(7.0, element, noImpulse element)

        Assert.AreEqual(21.0, resultingElement.Location.X)

    [<Test>] member TestTime.ElementIsMovingAlongXY_TimeTicks_ElementMovesAlongXY() = 
        let velocity = Velocity(Vector(3.0, 5.0))
        let element = {element with Velocity = velocity}
        
        let resultingElement = Time.Tick(7.0, element, noImpulse element)

        Assert.AreEqual(21.0, resultingElement.Location.X)
        Assert.AreEqual(35.0, resultingElement.Location.Y)


    [<Test>] member TestTime.ElementIsStationary_TimeTicks_ElementDoesntMove() = 
        let resultingElement = Time.Tick(7.0, element, noImpulse element)
        Assert.AreEqual(0.0, resultingElement.Location.X)
        Assert.AreEqual(0.0, resultingElement.Location.Y)

    [<Test>] member TestTime.ElementIsStationary_Push_ElementStartsToMove() = 
        let pi = PendingImpulse(element, Vector(15.0, 35.0))

        let resultingElement = Time.Tick(7.0, element, pi)
            
        Assert.AreEqual(3.0, resultingElement.Velocity.Vector.X)
        Assert.AreEqual(7.0, resultingElement.Velocity.Vector.Y)