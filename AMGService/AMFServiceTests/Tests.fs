namespace AMFServiceTests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open AMFService
[<TestClass>]
type TestClass () =
    [<TestMethod>]
    member this.TestMethodPassing () =
        
        Assert.AreEqual("Hello: Adam", "Hello: Adam");
