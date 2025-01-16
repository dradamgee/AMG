﻿namespace AMFService

open System
open System.IO
open System.Collections.Generic
open Microsoft.FSharp.Collections
open System.Text.Json

type StreamID = StreamID of int

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
    abstract member CreateOrderFromFile: string -> Async<DAL<'T> * OrderID * string * BlockOrder option>
            
    
module FileReader = 
    let encoding = System.Text.Encoding.UTF8
    let GetIDfromFileName (fileName:string) =                 
        OrderID (System.Int32.Parse (Path.GetFileNameWithoutExtension(fileName)))
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

type OrderStoreActor<'T>(dal:DAL<'T>, orderID:OrderID, rootPath:string, initialState) =
    let (OrderID orderIDint) = orderID 
    let filePath = rootPath + orderIDint.ToString() + ".txt"    
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
   
    //let dropIdleJsonFileHandle (inbox:MailboxProcessor<orderPlayerMessage>) (streamWriter:StreamWriter) = 
    //    match inbox.CurrentQueueLength with 
    //        | 0 ->     
    //            streamWriter.Flush()
    //            streamWriter.Dispose()
    //            None
    //        | _ ->
    //            Some streamWriter

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
    member this.OrderID = orderID
    member this.StorePath = rootPath



type OrderStore<'T> (rootPath:string, dal:DAL<'T>)=     
    let pathExists = Directory.Exists(rootPath)    
    let actorList = 
        match pathExists with
            | false -> 
                Directory.CreateDirectory(rootPath) |> ignore
                []
            | true -> 
                    FileReader.LoadFromFolder<'T>(dal, rootPath) 
                    |> Seq.toList 
                    |> List.map OrderStoreActor
    
    let orderIDToActor = Dictionary<OrderID, OrderStoreActor<'T>>(actorList |> List.map(fun oa -> KeyValuePair(oa.OrderID, oa)))

    let placeIDToActor = Dictionary<PlaceID, OrderStoreActor<'T>>(List.concat (actorList |> List.map(fun oa -> (oa, oa.GetOrderSync 100))
                                     |> List.map(
                                        fun oo -> 
                                            match oo with 
                                                | (_, None) -> []
                                                | (osa, Some bo) -> (bo.Placements |> List.map(fun placement -> KeyValuePair(placement.PlaceID, osa) )))
                                     ))
        
    member this.Submit(submitEvent:SubmitEvent) =
        task {
            let actor = OrderStoreActor<'T>(dal, submitEvent.OrderID, rootPath, None)
            actor.WriteEvent(Submit submitEvent)
            orderIDToActor.Add(submitEvent.OrderID, actor) //todo fix this mutability. also each stream/actor could have multiple orders in a block.
            return actor
        }
    member this.Place(orderID, placeEvent:PlaceEvent) = 
        task {
            let actor = orderIDToActor[orderID]
            actor.WriteEvent(Place placeEvent)
            placeIDToActor.Add(placeEvent.PlaceID, actor)
            return placeEvent.PlaceID
        }
    member this.Fill(placeID, fillEvent:FillEvent) = 
        let actor = orderIDToActor[placeID]
        actor.WriteEvent(Fill fillEvent)
        actor
    member this.GetActor(orderID) = orderIDToActor[orderID]
    member this.GetOrder(orderID) = 
        match orderIDToActor[orderID].TryGetOrder(1000000) with 
            | Some order -> order
            | None -> failwith "can't find order"            
    member this.GetOrderSync(orderID) = 
        match orderIDToActor[orderID].GetOrderSync(1000000) with             
            | Some order -> order
            | None -> failwith "can't find order"
        





