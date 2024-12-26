namespace AMFService

open System
open System.IO
open System.Collections.Generic
open Microsoft.FSharp.Collections
open System.Text.Json

type orderReaderMessage = 
    | Get of AsyncReplyChannel<BlockOrder option>
    | Set of BlockOrder option

type orderPlayerMessage = 
    | GetOrderState of AsyncReplyChannel<BlockOrder option>
    | Play of OrderEvent

type DAL<'T> = 
    abstract member FileAccess: string -> 'T
    abstract member WriteEventToFile: OrderEvent * 'T -> unit
    abstract member DropIdleFileHandle: MailboxProcessor<orderPlayerMessage> -> 'T -> 'T option
    abstract member CreateOrderFromFile: string -> Async<DAL<'T> * int * string * BlockOrder option>
            
    
module FileReader = 
    let encoding = System.Text.Encoding.UTF8
    let GetIDfromFileName (fileName:string) =                 
        System.Int32.Parse (Path.GetFileNameWithoutExtension(fileName))
        //try        //    Some (System.Int32.Parse (Path.GetFileNameWithoutExtension(fileName)))        //with _ -> None

    let rec CreateOrderFromEvents (events:IEnumerator<OrderEvent>, blockOrder:BlockOrder option) = 
        match (events.MoveNext(), blockOrder) with 
            | (true, blockOrder) -> CreateOrderFromEvents (events, Some (OrderEventPlayer.play (blockOrder, events.Current)))
            | (false, blockOrder) -> blockOrder
            
    let LoadFromFolder<'T>(dal: DAL<'T>, path:string) = 
        Directory.GetFiles(path)
        |> Seq.map dal.CreateOrderFromFile
        |> Async.Parallel
        |> Async.RunSynchronously
        //|> Seq.filter(fun (_, _, orderOption) -> orderOption.IsSome)        







type OrderStoreActor<'T>(dal:DAL<'T>, id:int, rootPath:string, initialState) =
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
    
    

    let dropIdleJsonFileHandle (inbox:MailboxProcessor<orderPlayerMessage>) (streamWriter:StreamWriter) = 
        match inbox.CurrentQueueLength with 
            | 0 ->     
                streamWriter.Flush()
                streamWriter.Dispose()
                None
            | _ ->
                Some streamWriter

    let eventPlayerAgentBuilder (dal: DAL<'T>) = MailboxProcessor.Start(fun inbox ->
            let difh = dal.DropIdleFileHandle inbox
            let rec messageLoop (orderState: BlockOrder option, fileAccessOption: 'T option) = 
                async{
                    let! msg = inbox.Receive()
                    match msg with 
                        | orderPlayerMessage.GetOrderState arc ->   
                            let fileAccess = match fileAccessOption with                                                 
                                                                 | None -> None
                                                                 | Some bw -> difh bw
                            arc.Reply(orderState)
                            return! messageLoop (orderState, fileAccess)
                        | orderPlayerMessage.Play orderEvent -> 
                            let fileAccess = match fileAccessOption with                                 
                                                    | None -> 
                                                        dal.FileAccess filePath                                                        
                                                    | Some bw -> bw
                            dal.WriteEventToFile (orderEvent, fileAccess) |> ignore //
                            let newState = OrderEventPlayer.play(orderState, orderEvent)
                            orderReaderAgent.Post(Set (Some newState))
                            return! messageLoop (Some newState, difh fileAccess)
                }
            messageLoop (initialState, None)
        )
    
    let jsonFileAccess (path:string) = File.AppendText(path)

    

    let orderEventMailbox = eventPlayerAgentBuilder(dal)
    
    member this.WriteEvent (orderEvent) = orderEventMailbox.Post (Play orderEvent)
    member this.GetOrderSync(timeout) = orderEventMailbox.PostAndReply ((fun arc -> GetOrderState arc), timeout)
    member this.GetOrderSync1(timeout) = orderEventMailbox.PostAndAsyncReply ((fun arc -> GetOrderState arc), timeout) // todo use async reply
    member this.TryGetOrder (timeout) = orderReaderAgent.PostAndReply((fun arc -> Get arc), timeout)
    member this.ID = id
    member this.StorePath = rootPath



type OrderStore<'T> (rootPath:string, dal:DAL<'T>)=     
    let pathExists = Directory.Exists(rootPath)    
    let actorList = 
        match pathExists with
            | false -> 
                Directory.CreateDirectory(rootPath) |> ignore
                []
            | true -> 
                    let asd = FileReader.LoadFromFolder<'T>(dal, rootPath) 
                    asd |> Seq.toList 
                      |> List.map OrderStoreActor
    
    let actorDictionary = Dictionary<int, OrderStoreActor<'T>>(actorList |> List.map(fun oa -> KeyValuePair(oa.ID, oa)))
        
    member this.Submit(submitEvent:SubmitEvent) =
        task {
            let actor = OrderStoreActor<'T>(dal, submitEvent.OrderID, rootPath, None)
            actor.WriteEvent(Submit submitEvent)
            actorDictionary.Add(submitEvent.OrderID, actor) //todo fix this mutability. also each stream/actor could have multiple orders in a block.
            return actor
        }
    member this.Place(id:int, placeEvent:PlaceEvent) = 
        let actor = actorDictionary[id]
        actor.WriteEvent(Place placeEvent)
        actor
    member this.Fill(id:int, tradeEvent:FillEvent) = 
        let actor = actorDictionary[id]
        actor.WriteEvent(Fill tradeEvent)
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
        





