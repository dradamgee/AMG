namespace AMG.Lottery.Test

open NUnit.Framework
open AMG.Lottery


[<TestFixture>]
type TestDraw () = 
    let draw = Draw([1uy; 2uy; 3uy; 4uy; 5uy; 6uy])    
    let losingTicket = Ticket([10uy; 20uy; 30uy; 40uy; 50uy])        
    let winningTicket = Ticket([1uy; 2uy; 3uy; 4uy; 5uy; 6uy])            
    let tickets = [losingTicket; winningTicket]

    [<Test>]
    member TestDraw.TicketMatches1Number_IsNotWinner() =        
        let winnings = draw.IsWinner(losingTicket)    
        Assert.AreEqual(WinningMatch.Nought, winnings)

    [<Test>]
    member TestDraw.TicketMatches1Number_IsWinner() =        
        let winnings = draw.IsWinner(winningTicket)    
        Assert.AreEqual(WinningMatch.Jackpot, winnings)
  
//    [<Test>]
//    member TestDraw.OneWinnerGetsWholePrizePot() =         
//        let stats = draw.GetStats(tickets)              
//        Assert.AreEqual(1, stats.JackpotWinnerCount)