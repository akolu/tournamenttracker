namespace Tournament

type Pairing =
    { Number: int
      Player1: string
      Player2: string
      Player1Score: int
      Player2Score: int }
    member this.IsScored = not (this.Player1Score = 0 && this.Player2Score = 0)
