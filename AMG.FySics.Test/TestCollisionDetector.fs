module TestCollisionDetector

open NUnit.Framework
open AMG.FySics
  
type elfa = 
    | Id of int
    | Location of Vector
    | Velocity of Velocity
    | Mass of float
    | Radius of double

[<TestFixture>]
type TestCollisionDetection() = 
      
    let an_element (stuff : elfa list) = 
        let mutable id = 0
        let mutable location = Vector(11.0, 0.0)
        let mutable velocity = VelocityModule.stationary
        let mutable mass = 1.0
        let mutable radius = 7.0

        let foo item = match item with
                | Id(x) -> do id <- x
                | Location(x) -> do location <- x
                | Velocity(x) -> do velocity <- x
                | Mass(x) -> do mass <- x
                | Radius(x) -> do radius <- x
        
        stuff |> 
            List.iter(foo)
                
        Element(id, location, velocity, mass, radius)
        
    let with_size value (parms : elfa list) = elfa.Radius(value) :: parms
    let with_location(value) (parms : elfa list) = elfa.Location(value) :: parms
    let with_velocity(value) (parms : elfa list) = elfa.Velocity(value) :: parms
    let with_mass(value) (parms : elfa list) = elfa.Mass(value) :: parms
    let with_radius(value) (parms : elfa list) = elfa.Location(value) :: parms
        
    [<Test>] member TestCollisionDetection.ItemsAreOverlaying_detect_IsTrue()=
        let element1 = [] |> with_size 1.0 |> an_element 
        let element2 = [] |> with_size 1.0 |> an_element 

        let result = CollisionDetector.overlap(element1, element2)
        Assert.IsTrue(result)

