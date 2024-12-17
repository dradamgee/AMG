namespace AMFServiceBinaryDAL
open AMFService
open System
open System.IO

type BinaryDAL() =    
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
    let GetIDfromFileName (fileName:string) =                 
        System.Int32.Parse (Path.GetFileNameWithoutExtension(fileName))
        //try        //    Some (System.Int32.Parse (Path.GetFileNameWithoutExtension(fileName)))        //with _ -> None
    let nextEventType(reader: IO.BinaryReader) = 
        try reader.ReadInt32() with
            | :? System.IO.EndOfStreamException as _ -> 0
    let ExractBinaryEvents (binaryReader:IO.BinaryReader, id:int) = 
            let rec ExtractEventLoop(reader:IO.BinaryReader, eventType) =             
                seq {
                    match eventType with    
                                  | 0 -> ignore 
                                  | 1 -> yield OrderEvent.Submit (DeserializeSubmitEvent(reader))
                                         yield! ExtractEventLoop(reader, nextEventType(reader))
                                  | 2 -> yield OrderEvent.Trade (DeserializeTradeEvent(reader))    
                                         yield! ExtractEventLoop(reader, nextEventType(reader))                                         
                }    
            ExtractEventLoop (binaryReader, nextEventType(binaryReader))

    interface DAL<IO.BinaryWriter> with
        member this.FileAccess (path:string) = 
            let filestream = IO.File.Create(path)
            new IO.BinaryWriter(filestream)
        member this.WriteEventToFile (orderEvent, binaryWriter:BinaryWriter) = 
            match orderEvent with
            | Submit submitEvent -> 
                //let eventImage = Serialize(submitEvent);                
                //streamWriter.Write(0 + eventImage) |> ignore
                binaryWriter.Write(1) |> ignore                               
                SerializeSubmitEvent(binaryWriter, submitEvent)
            | Trade tradeEvent -> 
                //let eventImage = Serialize(tradeEvent);                
                //streamWriter.WriteLine("1" + eventImage) |> ignore
                binaryWriter.Write(2) |> ignore                               
                SerializeTradeEvent(binaryWriter, tradeEvent)
            | Unknown -> () |> ignore
        member this.DropIdleFileHandle (inbox:MailboxProcessor<orderPlayerMessage>) (binaryWriter:IO.BinaryWriter) = 
            match inbox.CurrentQueueLength with 
            | 0 ->     
                binaryWriter.Flush()
                binaryWriter.Dispose()
                None
            | _ ->
                Some binaryWriter
        member this.CreateOrderFromFile (fileName:string) = 
            async {
                let fso = new FileStreamOptions()
                fso.Access <- FileAccess.Read
                fso.BufferSize  <- 4096
                use filestream = new IO.FileStream(fileName, fso)        
                use reader = new IO.BinaryReader(filestream)
               
                let id = GetIDfromFileName fileName
                let events = ExractBinaryEvents (reader, id) 
                return 
                    (this, id, fileName, FileReader.CreateOrderFromEvents(events.GetEnumerator(), None))
            }   


