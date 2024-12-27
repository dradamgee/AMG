namespace AMFServiceBinaryDAL
open AMFService
open System
open System.IO

module BinarySerializer =   
    let SerializeDecimal(writer:IO.BinaryWriter, value:Decimal) =         
        writer.Write(value)
    let SerializeInt32(writer:IO.BinaryWriter, value:int) =         
        writer.Write(value)
    let SerializeString(writer:IO.BinaryWriter, value:string) = 
        writer.Write(value)
    let SerializeSubmitEvent(writer, {OrderID=OrderID orderID; Size=size; Side=side; Asset=asset}) =
        SerializeInt32 (writer, int orderID)
        SerializeDecimal (writer, size)
        SerializeInt32 (writer, int side)
        SerializeString (writer, asset)
    let SerializePlaceEvent(writer, {PlaceID=PlaceID placeID; Size=size; CounterpartyID=counterpartyID}) = 
        SerializeInt32 (writer, placeID)
        SerializeDecimal (writer, size)
        SerializeInt32 (writer, counterpartyID)
    let SerializeFillEvent(writer, {PlaceID=PlaceID placeID; Size=size; Price=price}) =
        SerializeInt32 (writer, placeID)
        SerializeDecimal (writer, size)
        SerializeDecimal (writer, price)        
    let DeserializeSubmitEvent(reader:IO.BinaryReader) = 
        let orderID = reader.ReadInt32()
        let size = reader.ReadDecimal()
        let side = reader.ReadInt32()
        let asset = reader.ReadString()
        {OrderID=OrderID orderID; Size=size; Side=enum<Side> side; Asset=asset}
    let DeserializePlaceEvent(reader:IO.BinaryReader) = 
        let placeID = reader.ReadInt32()
        let size = reader.ReadDecimal()
        let counterpartyID = reader.ReadInt32()
        {PlaceID=PlaceID placeID; Size=size; CounterpartyID=counterpartyID}
    let DeserializeFillEvent(reader:IO.BinaryReader) =
        let placeID = reader.ReadInt32()
        let size = reader.ReadDecimal()
        let price = reader.ReadDecimal()
        {PlaceID=PlaceID placeID; Size=size; Price=price}

type BinaryDAL() =    
    let ExractEvents (binaryReader:IO.BinaryReader, id:int) = 
            let nextEventType(reader: IO.BinaryReader) = 
                try reader.ReadInt32() with
                | :? System.IO.EndOfStreamException as _ -> 0

            let rec ExtractEventLoop(reader:IO.BinaryReader, eventType) =             
                seq {
                    match eventType with    
                                  | 0 -> ignore 
                                  | 1 -> yield OrderEvent.Submit (BinarySerializer.DeserializeSubmitEvent(reader))
                                         yield! ExtractEventLoop(reader, nextEventType(reader))
                                  | 2 -> yield OrderEvent.Place (BinarySerializer.DeserializePlaceEvent(reader))
                                         yield! ExtractEventLoop(reader, nextEventType(reader))                                                         
                                  | 3 -> yield OrderEvent.Fill (BinarySerializer.DeserializeFillEvent(reader))    
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
                binaryWriter.Write(1) |> ignore                               
                BinarySerializer.SerializeSubmitEvent(binaryWriter, submitEvent)
            | Place placeEvent ->                 
                binaryWriter.Write(2) |> ignore                               
                BinarySerializer.SerializePlaceEvent(binaryWriter, placeEvent)
            | Fill fillEvent ->                 
                binaryWriter.Write(3) |> ignore                               
                BinarySerializer.SerializeFillEvent(binaryWriter, fillEvent)
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
               
                let id = FileReader.GetIDfromFileName fileName
                let events = ExractEvents (reader, id) 
                return 
                    (this, id, fileName, FileReader.CreateOrderFromEvents(events.GetEnumerator(), None))
            }   


