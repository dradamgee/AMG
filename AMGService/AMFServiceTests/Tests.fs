namespace AMFServiceTests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open AMFService
open Hyper

type Side = | Buy = 0 | Sell = 1

type SubmitEvent = {Size:decimal; Side:Side; Asset:string}

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
    let DeserializeSubmitEvent(reader:IO.BinaryReader) = 
        let size = reader.ReadDecimal()
        let side = reader.ReadInt32()
        let asset = reader.ReadString()
        {Size=size; Side=enum<Side> side; Asset=asset}

type SubmitEventClass(Size:decimal, Side:int, Asset:string) = 
    member this.Size = Size
    member this.Side = Side
    member this.Asset = Asset


[<TestClass>]
type TestClass () =
    [<TestMethod>]
    member this.TestBinaryWriter () =
        
        let directoryPath = @"c:\AMG\" + Guid.NewGuid().ToString() 

        System.IO.Directory.CreateDirectory(directoryPath);

        let filePath = directoryPath + @"\TEST1.txt";

                        
        use filestream = IO.File.Create(filePath)
        use writer = new IO.BinaryWriter(filestream)
        
        for i in [1..1000000] do 
            EventSerializer.SerializeSubmitEvent(writer, {Size=10m; Side=enum<Side> i; Asset="Hello: Adam"})
        
        writer.Flush()
        filestream.Flush()
        filestream.Dispose()       
        
        use filestream2 = new IO.FileStream(filePath, IO.FileMode.Open)

        //filestream2.Position <- 0
        use reader = new IO.BinaryReader(filestream2)
        for i in [1..1000000] do 
        
            EventSerializer.DeserializeSubmitEvent(reader) |> ignore

            //Assert.AreEqual(readSize, 10m);
            //Assert.AreEqual(readSide, 1);
            //Assert.AreEqual(readAsset, "Hello: Adam");

    //[<TestMethod>]
    member this.TestHyperSerializer () =
        
        let directoryPath = @"c:\AMG\" + Guid.NewGuid().ToString() 
        System.IO.Directory.CreateDirectory(directoryPath);
        let filePath = directoryPath + @"\TEST1.txt";
        use filestream = IO.File.Create(filePath)
        use writer = new IO.BinaryWriter(filestream)
        
        Hyper.HyperSerializer.Serialize()

        //for i in [1..1000000] do             
        //let myEvent = {Size=10m; Side=1; Asset="Hello: Adam"}
        let myEvent = SubmitEventClass(10m, 1, "Hello: Adam")
        writer.Write(Hyper.HyperSerializer.Serialize(myEvent))
            
            //EventSerializer.SerializeSubmitEvent(writer, {Size=10m; Side=i; Asset="Hello: Adam"})
        
        writer.Flush()
        filestream.Flush()
        filestream.Dispose()       
        
        //use filestream2 = new IO.FileStream(filePath, IO.FileMode.Open)

        //filestream2.Position <- 0
        //use reader = new IO.BinaryReader(filestream2)
        //for i in [1..1000000] do 
        
            //EventSerializer.DeserializeSubmitEvent(reader) |> ignore

            //Assert.AreEqual(readSize, 10m);
            //Assert.AreEqual(readSide, 1);
            //Assert.AreEqual(readAsset, "Hello: Adam");

