namespace AMFService

open System.IO
open System.Collections.Generic
open System.Text.Json
type Side = | Buy = 0 | Sell = 1
//type ID = | ID of int

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
            
    
module FileReader = 
    let ExractEvents (fileName:string, id:int) = 
        File.ReadLines(fileName)
        |> Seq.map (fun readLine -> (readLine[0], readLine.Substring(1)))
        |> Seq.map (fun readLineTuple  -> match readLineTuple with 
                                          | ('0', serializedEvent) -> OrderEvent.Submit (JsonSerializer.Deserialize<SubmitEvent>(serializedEvent))
                                          | ('1', serializedEvent) -> OrderEvent.Trade (JsonSerializer.Deserialize<TradeEvent>(serializedEvent))                                      
                                          | (_, _) -> OrderEvent.Unknown
                    )

    let GetIDfromFileName (fileName:string) =                 
        System.Int32.Parse (Path.GetFileNameWithoutExtension(fileName))
        //try
        //    Some (System.Int32.Parse (Path.GetFileNameWithoutExtension(fileName)))
        //with _ -> None
    
    let rec CreateOrderFromEvents (events:IEnumerator<OrderEvent>, equityOrder:EquityOrder option) = 
        match (events.MoveNext(), equityOrder) with 
            | (true, equityOrder) -> CreateOrderFromEvents (events, Some (OrderEventPlayer.play (equityOrder, events.Current)))
            | (false, equityOrder) -> equityOrder
            
    let CreateOrderFromFile (fileName:string) = 
        let id = GetIDfromFileName fileName
        let events = ExractEvents (fileName, id) 
        (id, fileName, CreateOrderFromEvents(events.GetEnumerator(), None))

    let LoadFromFolder(path:string) = 
        Directory.GetFiles(path) 
        |> Seq.map(CreateOrderFromFile) 
        //|> Seq.filter(fun (_, _, orderOption) -> orderOption.IsSome)        

    let WriteEventToFile(orderEvent, streamWriter:StreamWriter) = 
        match orderEvent with
            | Submit submitEvent -> 
                let eventImage = JsonSerializer.Serialize(submitEvent);                
                streamWriter.WriteLine("0" + eventImage) |> ignore
            | Trade tradeEvent -> 
                let eventImage = JsonSerializer.Serialize(tradeEvent);                
                streamWriter.WriteLine("1" + eventImage) |> ignore
            | Unknown -> () |> ignore

type orderReaderMessage = 
    | Get of AsyncReplyChannel<EquityOrder option>
    | Set of EquityOrder option

type orderPlayerMessage = 
    | GetOrderState of AsyncReplyChannel<EquityOrder option>
    | Play of OrderEvent

type OrderStoreActor(id:int, rootPath:string, initialState) =
    let filePath = rootPath + id.ToString() + ".txt"    
    let orderReaderAgent = MailboxProcessor.Start(fun inbox ->
            let rec messageLoop (state) = async{
                let! msg = inbox.Receive()
                let newState = match msg with  
                                | Set setState -> setState
                                | Get asc -> 
                                    asc.Reply(state)
                                    state
                                
                return! messageLoop (newState)                
            }
            messageLoop (initialState)
            )
    
    let dropIdleFileHandle (inbox:MailboxProcessor<orderPlayerMessage>) (streamWriter:StreamWriter) = 
        match inbox.CurrentQueueLength with 
            | 0 ->     
                streamWriter.Flush()
                streamWriter.Dispose()
                None
            | _ ->
                Some streamWriter

    let eventPlayerAgent = MailboxProcessor.Start(fun inbox ->
            let difh = dropIdleFileHandle inbox
            let rec messageLoop (orderState: EquityOrder option, streamWriterOption: StreamWriter option) = 
                async{
                    let! msg = inbox.Receive()
                    match msg with 
                        | orderPlayerMessage.GetOrderState arc ->   
                            let asd = match streamWriterOption with                                                 
                                                                 | None -> None
                                                                 | Some sw -> difh sw
                            arc.Reply(orderState)
                            return! messageLoop (orderState, asd)
                        | orderPlayerMessage.Play orderEvent -> 
                            let streamWriter = match streamWriterOption with 
                                                    | None -> File.AppendText(filePath)
                                                    | Some sw -> sw
                            FileReader.WriteEventToFile(orderEvent, streamWriter) |> ignore
                            let newState = OrderEventPlayer.play(orderState, orderEvent)
                            orderReaderAgent.Post(Set (Some newState))
                            return! messageLoop (Some newState, difh streamWriter)
                }
            messageLoop (initialState, None)
        )

    member this.WriteEvent (orderEvent) = eventPlayerAgent.Post (Play orderEvent)
    member this.GetOrderSync(timeout) = eventPlayerAgent.PostAndReply ((fun arc -> GetOrderState arc), timeout)
    member this.GetOrderSync1(timeout) = eventPlayerAgent.PostAndAsyncReply ((fun arc -> GetOrderState arc), timeout) // todo use async reply
    member this.TryGetOrder (timeout) = orderReaderAgent.PostAndReply((fun arc -> Get arc), timeout)
    member this.ID = id
    member this.StorePath = rootPath


type OrderStore(rootPath:string) = 
    let pathExists = Directory.Exists(rootPath)    
    let actorList = 
        match pathExists with
            | false -> 
                Directory.CreateDirectory(rootPath) |> ignore
                []
            | true -> FileReader.LoadFromFolder(rootPath) 
                      |> Seq.toList 
                      |> List.map OrderStoreActor
    
    let actorDictionary = Dictionary<int, OrderStoreActor>(actorList |> List.map(fun oa -> KeyValuePair(oa.ID, oa)))

    member this.RootPath = rootPath
    member this.Submit(id:int, submitEvent:SubmitEvent) =
        task {
        let actor = OrderStoreActor(id, rootPath, None)
        actor.WriteEvent(Submit submitEvent)
        actorDictionary.Add(id, actor) //todo fix this mutability.
        return actor
        }
    member this.Trade(id:int, tradeEvent:TradeEvent) = 
        let actor = actorDictionary[id]
        actor.WriteEvent(Trade tradeEvent)
        actor
    member this.GetActor(id:int) = actorDictionary[id]
    member this.GetOrder(id:int) = 
        match actorDictionary[id].TryGetOrder(10000) with 
            | Some order -> order
            | None -> failwith "can't find order"            
    member this.GetOrderSync(id:int) = 
        match actorDictionary[id].GetOrderSync(10000) with             
            | Some order -> order
            | None -> failwith "can't find order"
        

[<AllowNullLiteral>]
type OrderService(rootPath:string) = 
    let mutable nextID:int = 1 //todo fix this mutability.
    let orderStore = new OrderStore(rootPath)
    member this.Submit(submitEvent:SubmitEvent) = 
        task {
            let id = nextID
            let nextID = nextID + 1
            orderStore.Submit(id, submitEvent) |> ignore
            return id
        }
        
    member this.Trade(id, submitEvent) = orderStore.Trade(id, submitEvent)

    member this.GetOrder(id) = orderStore.GetOrder(id)

    member this.GetOrderSync(ID:int) = orderStore.GetOrderSync(ID)



