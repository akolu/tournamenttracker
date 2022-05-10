module Tournament.Pairing

open Player

type Pairing =
    { Number: int
      Player1: Player
      Player2: Player
      Player1Score: int
      Player2Score: int }
    member this.IsScored = not (this.Player1Score = 0 && this.Player2Score = 0)

    member this.Score result =
        { this with
            Player1Score = fst result
            Player2Score = snd result }
