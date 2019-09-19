namespace AMG.Mafs

// let r = 1.0
// let newc = ( hcs + ((r - (((r ** 2.0) - hcs) ** 0.5)) ** 2.0) ) ** 0.5

type LifeOf() = 
    member this.bisect ((i: float), (c : float)) = 
        let hcs = (c / 2.0) ** 2.0
        let newc = ( hcs + ((1.0 - (((1.0) - hcs) ** 0.5)) ** 2.0) ) ** 0.5
        let newi = i * 2.0
        let pi = i * newc
        (pi, (newi, newc))
    member this.GregoryLeibniz ((seriesSum: double), (sign: bool), (divisor: double)) =
        let nextDivisor = divisor + 2.0
        let nextSign = not sign
        if sign then
          let newSeriesSum = seriesSum + (1.0 / divisor)
          (newSeriesSum * 4.0, (newSeriesSum, nextSign, nextDivisor))
        else
          let newSeriesSum = seriesSum - (1.0 / divisor)
          (newSeriesSum * 4.0, (newSeriesSum, nextSign, nextDivisor))
    // start with 3.0 + 2.0
    member this.Nilakantha ((seriesSum: double), (sign: bool), (divisorSeed: double)) =
        let divisor = divisorSeed * (divisorSeed + 1.0) * (divisorSeed + 2.0)
        let nextDivisorSeed = divisorSeed + 2.0
        let nextSign = not sign
        if sign then
          let newSeriesSum = seriesSum + (4.0 / divisor)
          (newSeriesSum, (newSeriesSum, nextSign, nextDivisorSeed))
        else
          let newSeriesSum = seriesSum - (4.0 / divisor)
          (newSeriesSum, (newSeriesSum, nextSign, nextDivisorSeed))


