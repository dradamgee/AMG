namespace AMFService
open AMFServiceBinaryDAL
open AMFServiceJsonDAL
open AMFService
open System.IO

type OrderServiceMode = 
        | JsonMode = 1
        | BinaryMode = 2

type OrderStoreDU = 
        | Json of OrderStore<StreamWriter>
        | Binary of OrderStore<BinaryWriter>

type OrderService (rootPath:string, mode:OrderServiceMode) = 
    let mutable nextID:int = 1 //todo fix this mutability.

    let orderStore = 
        match mode with 
        | OrderServiceMode.JsonMode -> Json (OrderStore<_> (rootPath, JsonDAL()))
        | OrderServiceMode.BinaryMode -> Binary (OrderStore<_> (rootPath, BinaryDAL()))
    
    
    member this.Submit(submitCommand:SubmitCommand) = 
        task {
            let id = OrderID nextID
            nextID <- nextID + 1

            let submitEvent = {OrderID=id; Size=submitCommand.Size; Side=submitCommand.Side; Asset=submitCommand.Asset}

            match orderStore with 
                   | Json orderStore -> orderStore.Submit(submitEvent) |> ignore
                   | Binary orderStore -> orderStore.Submit(submitEvent) |> ignore
            
            return id // TODO separate the stream id from the order id.
        }

    member this.Place(placeCommand:PlaceCommand) =  
        task {
            let id = PlaceID nextID
            nextID <- nextID + 1

            let placeEvent = {PlaceID=id; Size=placeCommand.Size; CounterpartyID=placeCommand.CounterpartyID}
        
            return match orderStore with 
                    | Json orderStore -> orderStore.Place(placeCommand.OrderID, placeEvent).Result
                    | Binary orderStore -> orderStore.Place(placeCommand.OrderID, placeEvent).Result
        }

    member this.Fill(id, submitEvent) = 
        match orderStore with 
        | Json orderStore -> orderStore.Fill(id, submitEvent) |> ignore
        | Binary orderStore -> orderStore.Fill(id, submitEvent) |> ignore
    
    member this.GetOrder(id) = 
        match orderStore with 
        | Json orderStore -> orderStore.GetOrder(id)
        | Binary orderStore -> orderStore.GetOrder(id)
    

    member this.GetOrderSync(orderID) = 
        match orderStore with 
        | Json orderStore -> orderStore.GetOrderSync(orderID)
        | Binary orderStore -> orderStore.GetOrderSync(orderID)

