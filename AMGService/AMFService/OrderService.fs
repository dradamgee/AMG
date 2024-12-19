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
    
    
    member this.Submit(submitEvent:SubmitEvent) = 
        task {
            let id = nextID
            nextID <- nextID + 1
            return match orderStore with 
                   | Json orderStore -> orderStore.Submit(id, submitEvent).Result.ID
                   | Binary orderStore -> orderStore.Submit(id, submitEvent).Result.ID
        }
        
    member this.Trade(id, submitEvent) = 
        match orderStore with 
        | Json orderStore -> orderStore.Trade(id, submitEvent) |> ignore
        | Binary orderStore -> orderStore.Trade(id, submitEvent) |> ignore
    
    member this.GetOrder(id) = 
        match orderStore with 
        | Json orderStore -> orderStore.GetOrder(id)
        | Binary orderStore -> orderStore.GetOrder(id)
    

    member this.GetOrderSync(ID:int) = 
        match orderStore with 
        | Json orderStore -> orderStore.GetOrderSync(ID)
        | Binary orderStore -> orderStore.GetOrderSync(ID)

