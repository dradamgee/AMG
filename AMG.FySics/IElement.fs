namespace AMG.FySics
    
type IElementxxx = 
    abstract member Id : int with get
    abstract member Location : Vector with get, set
    abstract member Velocity : Velocity with get, set
    abstract member Radius : float with get
    abstract member Mass : float with get        
    abstract member Top : float with get        
    abstract member Left : float with get        
    abstract member Bottom : float with get        
    abstract member Right : float with get   
        
type Element(id : int, location : Vector, velocity : Velocity, mass : float, radius : float ) = 
    member val Id = id
    member val Location = location
    member val Velocity = velocity
    member val Mass = mass
    member val Radius = radius
    
        
