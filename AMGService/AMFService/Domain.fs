namespace AMFService

type Side = | Buy = 0 | Sell = 1

type SubmitEvent = {Size:decimal; Side:Side; Asset:string}
type TradeEvent = {Size:decimal; Price:decimal}
type OrderEvent =
      | Submit of SubmitEvent
      | Trade of TradeEvent
      | Unknown


//type EquityOrderRECORD = {Size:decimal; Side:Side; Asset:string; TradedSize:decimal; TradedPrice:decimal}
type EquityOrder (size:decimal, side:Side, asset:string, tradeEvents:TradeEvent list, tradedSize:decimal, tradedPrice:decimal) = 
      new (size:decimal, side:Side, asset:string) = EquityOrder (size, side, asset, [], 0m, 0m)
      member this.Size = size
      member this.Side = side
      member this.Asset = asset      
      member this.TradeEvents = tradeEvents
      member this.TradedSize = tradedSize
      member this.TradedPrice = tradedPrice
      

module OrderEventPlayer =     
    let playSubmit ({Size=size; Side=side; Asset=asset}) = 
        EquityOrder(size, side, asset)
    let playTrade (equityOrder:EquityOrder, tradeEvent ) = 
        let newTradeEvents = tradeEvent :: equityOrder.TradeEvents
        let {Size=tradedSize; Price=tradedPrice} = tradeEvent
        let newTradedSize = tradedSize+equityOrder.TradedSize
        let newTradedPrice=((tradedPrice*tradedSize)+(equityOrder.TradedPrice*equityOrder.TradedSize))/(tradedSize+equityOrder.TradedSize)
        EquityOrder(equityOrder.Size, equityOrder.Side, equityOrder.Asset, newTradeEvents, newTradedSize, newTradedPrice)
    let play asd =         
        match asd with
            | (_, OrderEvent.Submit submitevent) -> playSubmit (submitevent)
            | (Some equityOrder, OrderEvent.Trade tradeEvent) -> playTrade (equityOrder, tradeEvent)
            | (None, OrderEvent.Trade _) -> failwith "Unsolicited Trades are not supported"


