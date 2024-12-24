namespace AMFService

type Side = | Buy = 0 | Sell = 1

type SubmitEvent = {Size:decimal; Side:Side; Asset:string}
type TradeEvent = {Size:decimal; Price:decimal}
type OrderEvent =
      | Submit of SubmitEvent
      | Trade of TradeEvent
      | Unknown

type EquityOrder (size:decimal, side:Side, asset:string) = 
      //new (size:decimal, side:Side, asset:string) = EquityOrder (size, side, asset, [], 0m, 0m)
      member this.Size = size
      member this.Side = side
      member this.Asset = asset      


type EquityPlacement (size:decimal) =
      member this.Size = size
      
type BlockOrder(orders, tradeEvents:TradeEvent list, tradedSize:decimal, tradedPrice:decimal) = 
      new (orders) = BlockOrder (orders, [], 0m, 0m)
      member this.Orders : EquityOrder list = orders
      member this.TradeEvents = tradeEvents
      member this.TradedSize = tradedSize
      member this.TradedPrice = tradedPrice

type EquityFill (size:decimal) = 
      member this.Size = size
      

module OrderEventPlayer =     
    let private playSubmit (blockOrder:BlockOrder option, {Size=size; Side=side; Asset=asset}) = 
        match blockOrder with 
        | None -> BlockOrder([EquityOrder(size, side, asset)])
        | Some bo -> BlockOrder(EquityOrder(size, side, asset) :: bo.Orders, bo.TradeEvents, bo.TradedSize, bo.TradedPrice)
        
    let private playTrade (blockOrder:BlockOrder, tradeEvent) = 
        let newTradeEvents = tradeEvent :: blockOrder.TradeEvents
        let {Size=tradedSize; Price=tradedPrice} = tradeEvent
        let newTradedSize = tradedSize+blockOrder.TradedSize
        let newTradedPrice=((tradedPrice*tradedSize)+(blockOrder.TradedPrice*blockOrder.TradedSize))/(tradedSize+blockOrder.TradedSize)
        BlockOrder(blockOrder.Orders, newTradeEvents, newTradedSize, newTradedPrice)
    let play asd =         
        match asd with
            | (bo, OrderEvent.Submit submitevent) -> playSubmit (bo, submitevent)
            | (Some blockOrder, OrderEvent.Trade tradeEvent) -> playTrade (blockOrder, tradeEvent)
            | (None, OrderEvent.Trade _) -> failwith "Unsolicited Trades are not supported"


