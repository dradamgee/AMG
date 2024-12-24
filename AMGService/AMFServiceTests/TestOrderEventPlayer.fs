﻿namespace AMFServiceTests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open AMFService


[<TestClass>]
type TestOrderEventPlayer () =
    
    [<TestMethod>]
    member this.TestPlaySubmit () = 
        let blockOrder = OrderEventPlayer.play(None, OrderEvent.Submit {Size=3m; Side=Side.Buy; Asset=""} )
        Assert.AreEqual(3m, blockOrder.Orders.Head.Size)

    [<TestMethod>]
    member this.TestPlaySubmit2Orders () = 
        let blockOrder = OrderEventPlayer.play(None, OrderEvent.Submit {Size=5m; Side=Side.Buy; Asset=""} )
        let blockOrder = OrderEventPlayer.play(Some blockOrder, OrderEvent.Submit {Size=7m; Side=Side.Buy; Asset=""} )
        let asd = List.fold (fun acc (order:EquityOrder) -> acc + order.Size) 0m blockOrder.Orders
        Assert.AreEqual(12m, asd )

        
