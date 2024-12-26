namespace AMFServiceTests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open AMFService


[<TestClass>]
type TestOrderEventPlayer () =
    
    [<TestMethod>]
    member this.TestPlaySubmit () = 
        let blockOrder = OrderEventPlayer.play(None, OrderEvent.Submit {OrderID=1; Size=3m; Side=Side.Buy; Asset=""} )
        Assert.AreEqual(3m, blockOrder.Orders.Head.Size)

    [<TestMethod>]
    member this.TestPlaySubmit2Orders () = 
        let blockOrder = OrderEventPlayer.play(None, OrderEvent.Submit {OrderID=1;Size=5m; Side=Side.Buy; Asset=""} )
        let blockOrder = OrderEventPlayer.play(Some blockOrder, OrderEvent.Submit {OrderID=2; Size=7m; Side=Side.Buy; Asset=""} )
        let asd = List.fold (fun acc (order:EquityOrder) -> acc + order.Size) 0m blockOrder.Orders
        Assert.AreEqual(12m, asd )

    [<TestMethod>]
    member this.TestPlaceSingleOrder () = 
        let blockOrder = OrderEventPlayer.play(None, OrderEvent.Submit {OrderID=1;Size=5m; Side=Side.Buy; Asset=""} )
        let blockOrder = OrderEventPlayer.play(Some blockOrder, OrderEvent.Place {PlaceID=1; Size=5m; CounterpartyID=1} )
        Assert.AreEqual(5m, blockOrder.Placements.Head.Size)

    [<TestMethod>]
    member this.TestPlaceAndFill () = 
        let blockOrder = OrderEventPlayer.play(None, OrderEvent.Submit {OrderID=1;Size=5m; Side=Side.Buy; Asset=""} )
        let blockOrder = OrderEventPlayer.play(Some blockOrder, OrderEvent.Place {PlaceID=1; Size=5m; CounterpartyID=1} )
        let blockOrder = OrderEventPlayer.play(Some blockOrder, OrderEvent.Fill {PlaceID=1; Size=5m; Price=7m} )
        Assert.AreEqual(5m, blockOrder.Placements.Head.FilledSize)
        Assert.AreEqual(7m, blockOrder.Placements.Head.FilledPrice)
        


        
