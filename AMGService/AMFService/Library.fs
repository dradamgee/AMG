namespace AMFService

open System
open System.IO
open System.Collections.Generic
open Microsoft.FSharp.Collections

//open System.Text.Json

type Side = | Buy = 0 | Sell = 1
//type ID = | ID of int

type SubmitEvent = {Size:decimal; Side:Side; Asset:string}
type TradeEvent = {Size:decimal; Price:decimal}
type OrderEvent =
      | Submit of SubmitEvent
      | Trade of TradeEvent
      | Unknown

module EventSerializer = 
    let SerializeDecimal(writer:IO.BinaryWriter, value:Decimal) =         
        writer.Write(value)
    let SerializeInt32(writer:IO.BinaryWriter, value:int) =         
        writer.Write(value)
    let SerializeString(writer:IO.BinaryWriter, value:string) = 
        writer.Write(value)
    let SerializeSubmitEvent(writer, {Size=size; Side=side; Asset=asset}) =
        SerializeDecimal (writer, size)
        SerializeInt32 (writer, int side)
        SerializeString (writer, asset)
    let SerializeTradeEvent(writer, {Size=size; Price=price}) =
        SerializeDecimal (writer, size)
        SerializeDecimal (writer, price)
    let DeserializeSubmitEvent(reader:IO.BinaryReader) = 
        let size = reader.ReadDecimal()
        let side = reader.ReadInt32()
        let asset = reader.ReadString()
        {Size=size; Side=enum<Side> side; Asset=asset}
    let DeserializeTradeEvent(reader:IO.BinaryReader) =
        let size = reader.ReadDecimal()
        let price = reader.ReadDecimal()
        {Size=size; Price=price}

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
    let encoding = System.Text.Encoding.UTF8
    
    let SerializeXXX<'T> (serializable: 'T) =                 
        //json
        System.Text.Json.JsonSerializer.Serialize(serializable)
        //Hyper
        //let mybytepan = Hyper.HyperSerializer.Serialize(serializable)
        //encoding.GetString(mybytepan.ToArray())        
        //
        //ProtoBuf
        //let stream = new MemoryStream()
        //ProtoBuf.Serializer.Serialize<'T>(stream, serializable)
        //let reader = new StreamReader( stream, System.Text.Encoding.UTF8);
        //reader.ReadToEnd()
    let Deserialize<'T>(serialized: string) : 'T = 
        //json
        System.Text.Json.JsonSerializer.Deserialize<'T>(serialized)        
        //Hyper
        //let bytearray = encoding.GetBytes(serialized)
        //let bytespan = System.Span<byte>(bytearray)
        //Hyper.HyperSerializer.Deserialize(bytespan)
        //ProtoBuf
        //ProtoBuf.Serializer.Deserialize<'T>(MemoryStream (System.Text.Encoding.UTF8.GetBytes(serialized)))       
    
        

    let nextEventType(reader: IO.BinaryReader) = 
        try reader.ReadInt32() with
            | :? System.IO.EndOfStreamException as _ -> 0
    
    let ExractEvents (binaryReader:IO.BinaryReader, id:int) = 
        let rec ExtractEventLoop(reader:IO.BinaryReader, eventType) =             
            seq {
                match eventType with    
                              | 0 -> ignore 
                              | 1 -> yield OrderEvent.Submit (EventSerializer.DeserializeSubmitEvent(reader))
                                     yield! ExtractEventLoop(reader, nextEventType(reader))
                              | 2 -> yield OrderEvent.Trade (EventSerializer.DeserializeTradeEvent(reader))    
                                     yield! ExtractEventLoop(reader, nextEventType(reader))                                         
            }    
        ExtractEventLoop (binaryReader, nextEventType(binaryReader))
        
        //let filestream = new IO.FileStream(fileName, IO.FileMode.Open)        
        //let reader = new IO.BinaryReader(filestream)
        //do reader.Dispose() 
        //do filestream.Dispose() 
        
    //let ExractEventsOld (fileName:string, id:int) = 
    //    File.ReadLines(fileName)
    //    |> Seq.map (fun readLine -> (readLine[0], readLine.Substring(1)))
    //    |> Seq.map (fun readLineTuple  -> match readLineTuple with 
    //                                      | ('0', serializedEvent) -> OrderEvent.Submit (Deserialize<SubmitEvent>(serializedEvent))
    //                                      | ('1', serializedEvent) -> OrderEvent.Trade (Deserialize<TradeEvent>(serializedEvent))                                      
    //                                      | (_, _) -> OrderEvent.Unknown
    //                )

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
        async {
            let fso = new FileStreamOptions()
            fso.Access <- FileAccess.Read
            fso.BufferSize  <- 4096
            use filestream = new IO.FileStream(fileName, fso)        
            use reader = new IO.BinaryReader(filestream)
               
            let id = GetIDfromFileName fileName
            let events = ExractEvents (reader, id) 
            return 
                (id, fileName, CreateOrderFromEvents(events.GetEnumerator(), None))
        }
        //do reader.Dispose() 
        //do filestream.Dispose() 


    let LoadFromFolder(path:string) = 
        Directory.GetFiles(path)
        |> Seq.map CreateOrderFromFile
        |> Async.Parallel
        |> Async.RunSynchronously
        //|> Seq.filter(fun (_, _, orderOption) -> orderOption.IsSome)        

    let WriteEventToFile(orderEvent, binaryWriter:BinaryWriter) = 
        match orderEvent with
            | Submit submitEvent -> 
                //let eventImage = Serialize(submitEvent);                
                //streamWriter.Write(0 + eventImage) |> ignore
                binaryWriter.Write(1) |> ignore                               
                EventSerializer.SerializeSubmitEvent(binaryWriter, submitEvent)
            | Trade tradeEvent -> 
                //let eventImage = Serialize(tradeEvent);                
                //streamWriter.WriteLine("1" + eventImage) |> ignore
                binaryWriter.Write(2) |> ignore                               
                EventSerializer.SerializeTradeEvent(binaryWriter, tradeEvent)

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
    
    let dropIdleFileHandle (inbox:MailboxProcessor<orderPlayerMessage>) (binaryWriter:IO.BinaryWriter) = 
        match inbox.CurrentQueueLength with 
            | 0 ->     
                binaryWriter.Flush()
                binaryWriter.Dispose()
                None
            | _ ->
                Some binaryWriter

    let eventPlayerAgent = MailboxProcessor.Start(fun inbox ->
            let difh = dropIdleFileHandle inbox
            let rec messageLoop (orderState: EquityOrder option, binaryWriterOption: BinaryWriter option) = 
                async{
                    let! msg = inbox.Receive()
                    match msg with 
                        | orderPlayerMessage.GetOrderState arc ->   
                            let binaryWriter = match binaryWriterOption with                                                 
                                                                 | None -> None
                                                                 | Some bw -> difh bw
                            arc.Reply(orderState)
                            return! messageLoop (orderState, binaryWriter)
                        | orderPlayerMessage.Play orderEvent -> 
                            let binaryWriter = match binaryWriterOption with 
                                                    | None -> let filestream = IO.File.Create(filePath)
                                                              new IO.BinaryWriter(filestream)
                                                    | Some bw -> bw
                            FileReader.WriteEventToFile(orderEvent, binaryWriter) |> ignore
                            let newState = OrderEventPlayer.play(orderState, orderEvent)
                            orderReaderAgent.Post(Set (Some newState))
                            return! messageLoop (Some newState, difh binaryWriter)
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
            | true -> 
                    let asd = FileReader.LoadFromFolder(rootPath) 
                    asd |> Seq.toList 
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
        match actorDictionary[id].TryGetOrder(1000000) with 
            | Some order -> order
            | None -> failwith "can't find order"            
    member this.GetOrderSync(id:int) = 
        match actorDictionary[id].GetOrderSync(1000000) with             
            | Some order -> order
            | None -> failwith "can't find order"
        

[<AllowNullLiteral>]
type OrderService(rootPath:string) = 
    let mutable nextID:int = 1 //todo fix this mutability.
    let orderStore = new OrderStore(rootPath)
    member this.Submit(submitEvent:SubmitEvent) = 
        task {
            let id = nextID
            nextID <- nextID + 1
            return orderStore.Submit(id, submitEvent).Result.ID
        }
        
    member this.Trade(id, submitEvent) = orderStore.Trade(id, submitEvent)

    member this.GetOrder(id) = orderStore.GetOrder(id)

    member this.GetOrderSync(ID:int) = orderStore.GetOrderSync(ID)



