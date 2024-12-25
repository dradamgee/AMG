namespace AMFService

type Side = | Buy = 0 | Sell = 1

type SubmitEvent = {Size:decimal; Side:Side; Asset:string}
type PlaceEvent = {PlaceID:int; Size:decimal; CounterpartyID:int}
type FillEvent = {PlaceID:int; Size:decimal; Price:decimal}
type OrderEvent =
      | Submit of SubmitEvent
      | Fill of FillEvent
      | Place of PlaceEvent
      | Unknown

type EquityOrder (size:decimal, side:Side, asset:string) = 
      //new (size:decimal, side:Side, asset:string) = EquityOrder (size, side, asset, [], 0m, 0m)
      member this.Size = size
      member this.Side = side
      member this.Asset = asset      

type EquityPlacement (placeEvent:PlaceEvent, fills: FillEvent list) =
      member this.PlaceID = placeEvent.Size
      member this.Size = placeEvent.Size
      member this.CounterpartyID = placeEvent.Size
      
type BlockOrder(orders,  placements:EquityPlacement list, fillEvents:FillEvent list, filledSize:decimal, filledPrice:decimal) = 
      new (orders) = BlockOrder (orders, [], [], 0m, 0m)
      member this.Orders : EquityOrder list = orders
      member this.PlacementEvents =  placements      
      member this.FillEvents = fillEvents      
      member this.FilledSize = filledSize
      member this.FilledPrice = filledPrice

      

module OrderEventPlayer =     
    let private playSubmit (blockOrder:BlockOrder option, {Size=size; Side=side; Asset=asset}) = 
        match blockOrder with 
        | None -> BlockOrder([EquityOrder(size, side, asset)])
        | Some bo -> BlockOrder(EquityOrder(size, side, asset) :: bo.Orders, bo.PlacementEvents, bo.FillEvents, bo.FilledSize, bo.FilledPrice)
        
    let private playPlace (blockOrder:BlockOrder, placeEvent) = 
        let newPlaceEvents = EquityPlacement(placeEvent, []) :: blockOrder.PlacementEvents
        BlockOrder(blockOrder.Orders, newPlaceEvents, blockOrder.FillEvents, blockOrder.FilledSize, blockOrder.FilledPrice)

    let private playFill (blockOrder:BlockOrder, fillEvent) = 
        let newFillEvents = fillEvent :: blockOrder.FillEvents
        let {Size=filledSize; Price=filledPrice} = fillEvent
        let newFilledSize = filledSize+blockOrder.FilledSize
        let newFilledPrice=((filledPrice*filledSize)+(blockOrder.FilledPrice*blockOrder.FilledSize))/(filledSize+blockOrder.FilledSize)
        BlockOrder(blockOrder.Orders, blockOrder.PlacementEvents, newFillEvents, newFilledSize, newFilledPrice)

    let play asd =         
        match asd with
            | (bo, OrderEvent.Submit submitevent) -> playSubmit (bo, submitevent)
            | (Some blockOrder, OrderEvent.Place placeEvent) -> playPlace (blockOrder, placeEvent)
            | (Some blockOrder, OrderEvent.Fill fillEvent) -> playFill (blockOrder, fillEvent)
            | (None, OrderEvent.Fill _) -> failwith "Unsolicited fills are not supported"


