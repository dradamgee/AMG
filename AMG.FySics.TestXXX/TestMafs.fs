namespace AMG.FySics.Test

open NUnit.Framework
open AMG.Mafs




[<TestFixture>]
type TestStuff() = 

    let piMaker = new LifeOf()   
    let thd(_, _, third) = third
     
    //[<Test>]
    member this.Test3() =                
        let myResults = 
          Seq.unfold
            ( fun state -> 
              if fst state < 100000.0
              then Some(piMaker.bisect state)
              else None 
            ) (2.0, 2.0)
        
        for result in myResults do printf "%A \n" result        
        Assert.AreEqual(3.1415924535897932, Seq.last myResults)

    

    //[<Test>]
    member this.Test_GregoryLeibniz() =                
        let myResults = 
          Seq.unfold
            ( fun state -> 
                let nextDivisor = state
                if (thd state > 1000000.0) then None
                else Some( piMaker.GregoryLeibniz state )
            ) (0.0, true, 1.0)
                
        Assert.AreEqual(3.1415924535897932, Seq.last myResults)


    [<Test>]
    member this.Test_Nilakantha() =                
        let myResults = 
          Seq.unfold
            ( fun state -> 
                let nextDivisor = state
                let seed = thd state
                if (seed > 100000000.0) then None
                else Some( piMaker.Nilakantha state )
            ) (3.0, true, 2.0)
          
        Assert.AreEqual(3.1415924535897932, Seq.last myResults)







