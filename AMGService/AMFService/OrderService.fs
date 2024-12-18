namespace AMFService
open AMFServiceBinaryDAL
open AMFServiceJsonDAL
open AMFService

[<AllowNullLiteral>]
type OrderService (rootPath:string) = 
    let mutable nextID:int = 1 //todo fix this mutability.
    //let orderStore = new OrderStore<_> (rootPath, BinaryDAL())
    let orderStore = new OrderStore<_> (rootPath, JsonDAL())
    member this.Submit(submitEvent:SubmitEvent) = 
        task {
            let id = nextID
            nextID <- nextID + 1
            return orderStore.Submit(id, submitEvent).Result.ID
        }
        
    member this.Trade(id, submitEvent) = orderStore.Trade(id, submitEvent)

    member this.GetOrder(id) = orderStore.GetOrder(id)

    member this.GetOrderSync(ID:int) = orderStore.GetOrderSync(ID)

