module TestCollision

open NUnit.Framework
open AMG.FySics

let areClose(expected : double, actual : double) = 
    let spread = 0.000000001
    Assert.IsTrue(abs(expected - actual) < spread, String.concat " " [actual.ToString(); "is not very equal to"; expected.ToString()])

let areClose2(expected: Vector , actual: Vector) =
        areClose(expected.X, actual.X)
        areClose(expected.Y, actual.Y)

let areApproximate (spread:double) (expected : double) (actual : double) = 
    Assert.IsTrue(abs(expected - actual) < spread, String.concat " " [actual.ToString(); "is not very equal to"; expected.ToString()])
      

let element(location:double, velocity:double) = 
        let e1_loc = Vector(location, 0.0)
        let foo = Vector(velocity, 0.0)
        let e1_vel = Velocity foo
        {Id = 1; Location = e1_loc; Velocity = e1_vel; Mass = 1.0; Radius = 10.0}

[<TestFixture>] 
type TestSoftSphereCollision()=
    [<Test>] member TestCollision.HeadOnCollision() = 
        let mutable e1 = element(100.0, -10.0)
        let mutable e2 = element(80.0, 10.0)
        let interval = 0.00001
        
        let mutable looping = true
        let mutable count = 0
        while looping do
            count<- count + 1
            let result = SoftSphereCollision(1.0).Act(e1,e2)
            let e1imp = match result with 
                | None -> PendingImpulse(e1, Vector(0.0, 0.0))
                | Some(x) -> PendingImpulse(e1, result.Value)
            let e2imp = match result with 
                | None -> PendingImpulse(e2, Vector(0.0, 0.0))
                | Some(x) -> PendingImpulse(e2, -result.Value)
            e1 <- Time.Tick(interval, e1, e1imp)
            e2 <- Time.Tick(interval, e2, e2imp)
            looping <-(e1.Location - e2.Location).Magnitude < 20.0
        
        System.Console.WriteLine(count)

        areApproximate 0.75 10.0 e1.Velocity.Vector.X 
        areApproximate 0.75 -10.0 e2.Velocity.Vector.X



[<TestFixture>] 
type TestHardSphereCollision()=
    [<Test>] member TestCollision.HeadOnCollision() = 
        let e1 = element(100.0, -10.0)
        let e2 = element(90.0, 10.0)

        let result = HardSphereCollision(1.0).Act(e1,e2)

        areClose2(Vector(20.0, 0.0), result.Value)

    [<Test>] member TestCollision.AfterHeadOnCollision() = 
        let e1 = element(100.0, 10.0)
        let e2 = element(90.0, -10.0)

        let result = HardSphereCollision(1.0).Act(e1,e2)

        Assert.IsNull(result)
    