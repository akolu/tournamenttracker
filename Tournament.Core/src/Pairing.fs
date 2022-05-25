module Tournament.Pairing

type Side =
    { Name: string
      PrimaryScore: int
      SecondaryScore: int }

type Pairing =
    { Number: int
      Player1: Side
      Player2: Side }
    member this.IsScored =
        not (
            this.Player1.PrimaryScore = 0
            && this.Player2.PrimaryScore = 0
        )

    member this.Score result =
        { this with
            Player1 = { this.Player1 with PrimaryScore = fst result }
            Player2 = { this.Player2 with PrimaryScore = snd result } }
