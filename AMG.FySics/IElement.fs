namespace AMG.FySics
    
    type IElement = 
        abstract member Id : int with get
        abstract member Location : Vector with get, set
        abstract member Velocity : Velocity with get, set
        abstract member Radius : float with get
        abstract member Mass : float with get        
        abstract member Top : float with get        
        abstract member Left : float with get        
        abstract member Bottom : float with get        
        abstract member Right : float with get   
