namespace AMFServiceTests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open AMFService


[<TestClass>]
type TestOrderEventPlayer () =
    
    [<TestMethod>]
    member this.TestPlaySbumit () = 
        let blockOrder = OrderEventPlayer.play(None, OrderEvent.Submit {Size=1m; Side=Side.Buy; Asset=""} )
        Assert.AreEqual(1m, blockOrder.Orders.Head.Size)

