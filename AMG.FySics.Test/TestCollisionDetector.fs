module TestCollisionDetector

open NUnit.Framework
open AMG.FySics

let SanityCheck(result : (Element * Element) list) =
        result |>
            List.iter (fun r -> 
                            match result.Head with
                                | (r1, r2) -> Assert.AreNotEqual(r1, r2)
                )
                  
[<TestFixture>]
type TestCollisionDetection() = 
    
    let an_element = {Element.Id = 0; Element.Location = Vector(0.0,0.0); Element.Velocity = Velocity(Vector(0.0,0.0)); Element.Mass = 0.0; Element.Radius = 0.0}
        
    let with_size value (element : Element) = {element with Radius = value}
    let with_location(value) (element : Element) = {element with Location = Vector(value, 0.0)}
    let with_velocity(value) (element : Element) = {element with Velocity = value}
    let with_mass(value) (element : Element) = {element with Mass = value}
    let with_radius(value) (element : Element) = {element with Location = value}
       
    [<Test>] member TestCollisionDetection.ItemsAreOverlaying_overlap_IsTrue()=
        let element1 = an_element |> with_size 1.0  
        let element2 = an_element |> with_size 1.0

        let result = CollisionDetector.overlap(element1, element2)
        Assert.IsTrue(result)

    [<Test>] member TestCollisionDetection.ItemsAreOverlapping_overlap_IsTrue()=
        let element1 = an_element |> with_size 7.0 |> with_location 3.0  
        let element2 = an_element |> with_size 5.0 |> with_location 13.0  
        
        let result = CollisionDetector.overlap(element1, element2)
        Assert.IsTrue(result)

    [<Test>] member TestCollisionDetection.ItemsNotOverlapping_overlap_IsFails()=
        let element1 = an_element |> with_size 7.0 |> with_location 3.0  
        let element2 = an_element |> with_size 5.0 |> with_location 17.0  
        
        let result = CollisionDetector.overlap(element1, element2)
        Assert.IsFalse(result)
    
    
    [<Test>] member TestCollisionDetection.TwoItems_overlap_OnePairReturned()=
        let element1 = an_element |> with_size 7.0 |> with_location 3.0  
        let element2 = an_element |> with_size 5.0 |> with_location 13.0  
        
        let result = CollisionDetector.detect([element1; element2])
        
        SanityCheck(result)
        
        Assert.AreEqual(1, result.Length)


 
