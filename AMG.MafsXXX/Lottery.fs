namespace AMG.Lottery

type WinningMatch = Nought | Consolation | Nice | Big | Jackpot

type DrawStats( loserCount: int, consolationCount: int, niceCount: int, bigCount: int, jackpotWinnerCount: int) =
    let ticketCost = 1.0
    let consolationPrize = 10.0
    let nicePrize = 100.0
    let bigPrize = 1000.0
    let prizeFundProportion = 0.5
    
    let totalCount = loserCount + consolationCount + niceCount + bigCount + jackpotWinnerCount
    let grossIncome = ticketCost * float(totalCount)
    let priceFund = prizeFundProportion * grossIncome
    let consolationPot = consolationPrize * float(consolationCount)
    let nicePot = nicePrize * float(niceCount)
    let bigPot = bigPrize * float(bigCount)
    let jackpot = prizeFundProportion - consolationPot - nicePot - bigPot
    let jackpotPrize = jackpot / float(jackpotWinnerCount)
    
    member FooStats.JackpotWinnerCount = jackpotWinnerCount
    
    


//6 numbers 1 and 59
type Ticket(numbers: List<byte>) = 
    member this.Nx = List.sort numbers
    
type Draw(numbers: List<byte>) = 
    let nx = List.sort numbers

    let rec getMatchCount(numbers: List<byte>) =        
        if numbers.IsEmpty then 0 else

        if List.exists (fun drawn -> drawn = numbers.[0] ) nx
        then getMatchCount(List.tail numbers) + 1
        else getMatchCount(List.tail numbers) 
    
    member Draw.IsWinner(ticket : Ticket) = 
        match getMatchCount(ticket.Nx) with         
        | 0 | 1 | 2 -> WinningMatch.Nought
        | 3 -> WinningMatch.Consolation
        | 4 -> WinningMatch.Nice
        | 5 -> WinningMatch.Big
        | 6 -> WinningMatch.Jackpot
        | _ -> WinningMatch.Nought
    
    member Draw.GetStats(tickets: List<Ticket>) =         
        Seq.countBy Draw.IsWinner tickets 



        
        
        
        
        

    

