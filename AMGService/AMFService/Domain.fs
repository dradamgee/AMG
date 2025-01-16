namespace AMFService


type Side = | Buy = 0 | Sell = 1



//type DomainID = 
//     | OrderID of OrderID
//     | PlaceID of PlaceID

type OrderID = OrderID of int
type PlaceID = PlaceID of int     


type SubmitCommand = {Size:decimal; Side:Side; Asset:string}
type SubmitEvent = {OrderID:OrderID; Size:decimal; Side:Side; Asset:string}

type PlaceCommand = {OrderID:OrderID; Size:decimal; CounterpartyID:int}
type PlaceEvent = {PlaceID:PlaceID; Size:decimal; CounterpartyID:int}
type FillEvent = {PlaceID:PlaceID; Size:decimal; Price:decimal}



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

type EquityPlacement (placeID, size, counterpartyID, fills: FillEvent list, filledSize : decimal, filledPrice:decimal) =
      new (placeEvent:PlaceEvent) = EquityPlacement(placeEvent.PlaceID, placeEvent.Size, placeEvent.Size, [], 0m, 0m)
      member this.PlaceID = placeID      
      member this.Size = size
      member this.CounterpartyID = counterpartyID
      member this.Fills = fills
      member this.FilledSize = filledSize
      member this.FilledPrice = filledPrice
      
type BlockOrder(orders,  placements:EquityPlacement list) = 
      new (orders) = BlockOrder (orders, [])
      member this.Orders : EquityOrder list = orders
      member this.Placements =  placements            
      //member this.FilledSize = filledSize
      //member this.FilledPrice = filledPrice

      

module OrderEventPlayer =     
    let private playSubmit (blockOrder:BlockOrder option, {OrderID=orderID;Size=size; Side=side; Asset=asset}) = 
        match blockOrder with 
        | None -> BlockOrder([EquityOrder(size, side, asset)])
        | Some bo -> BlockOrder(EquityOrder(size, side, asset) :: bo.Orders, bo.Placements)
        
    let private playPlace (blockOrder:BlockOrder, placeEvent) = 
        let newPlaceEvents = EquityPlacement(placeEvent) :: blockOrder.Placements
        BlockOrder(blockOrder.Orders, newPlaceEvents)

    let private playFill (blockOrder:BlockOrder, fillEvent) = 
        let {PlaceID=placeID; Size=filledSize; Price=filledPrice} = fillEvent
        let placement =  (blockOrder.Placements |> List.filter (fun asd -> asd.PlaceID = placeID)).Head
        let newFills = fillEvent :: placement.Fills
        let newFilledSize = filledSize+placement.FilledSize
        let newFilledPrice=((filledPrice*filledSize)+(placement.FilledPrice*placement.FilledSize))/(filledSize+placement.FilledSize)
        let newPlacement = EquityPlacement(placement.PlaceID, placement.Size, placement.CounterpartyID, newFills, newFilledSize, newFilledPrice)

        let newPlacements = newPlacement :: (blockOrder.Placements |> List.filter (fun asd -> asd.PlaceID <> placeID))

        BlockOrder(blockOrder.Orders, newPlacements)

    let play asd =         
        match asd with
            | (bo, OrderEvent.Submit submitevent) -> playSubmit (bo, submitevent)
            | (Some blockOrder, OrderEvent.Place placeEvent) -> playPlace (blockOrder, placeEvent)
            | (Some blockOrder, OrderEvent.Fill fillEvent) -> playFill (blockOrder, fillEvent)
            | (None, OrderEvent.Fill _) -> failwith "Unsolicited fills are not supported"


