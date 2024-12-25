namespace AMFServiceJsonDAL
open AMFService
open System
open System.IO
open System.Collections.Generic
open System.Text.Json

type JsonDAL() = 
    let ExractEvents (fileName:string, id:int) = 
        File.ReadLines(fileName)
        |> Seq.map (fun readLine -> (readLine[0], readLine.Substring(1)))
        |> Seq.map (fun readLineTuple  -> match readLineTuple with 
                                          | ('0', serializedEvent) -> OrderEvent.Submit (JsonSerializer.Deserialize<SubmitEvent>(serializedEvent))
                                          | ('1', serializedEvent) -> OrderEvent.Fill (JsonSerializer.Deserialize<FillEvent>(serializedEvent))                                      
                                          | (_, _) -> OrderEvent.Unknown
                    )
    interface DAL<StreamWriter> with
        
        member this.FileAccess (path:string) = 
            File.AppendText(path)
        member this.CreateOrderFromFile(fileName: string): Async<DAL<StreamWriter> * int * string * BlockOrder option> = 
            async{
                let id = FileReader.GetIDfromFileName fileName
                let events = ExractEvents (fileName, id) 
                return (this, id, fileName, FileReader.CreateOrderFromEvents(events.GetEnumerator(), None))
            }
        member this.DropIdleFileHandle(inbox: MailboxProcessor<orderPlayerMessage>) (streamWriter: StreamWriter): StreamWriter option = 
            match inbox.CurrentQueueLength with 
            | 0 ->     
                streamWriter.Flush()
                streamWriter.Dispose()
                None
            | _ ->
                Some streamWriter
        member this.WriteEventToFile(orderEvent: OrderEvent, streamWriter: StreamWriter): unit = 
            match orderEvent with
            | Submit submitEvent -> 
                let eventImage = JsonSerializer.Serialize(submitEvent);                
                streamWriter.WriteLine("0" + eventImage) |> ignore
            | Fill tradeEvent -> 
                let eventImage = JsonSerializer.Serialize(tradeEvent);                
                streamWriter.WriteLine("1" + eventImage) |> ignore
            | Unknown -> () |> ignore
        
            