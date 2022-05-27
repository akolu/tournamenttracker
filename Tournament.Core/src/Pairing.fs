module Tournament.Pairing

type Score =
    { Primary: int
      Secondary: int }
    static member Empty = { Primary = 0; Secondary = 0 }

    static member Of num = { Primary = num; Secondary = 0 }

    static member Of pair =
        { Primary = fst pair
          Secondary = snd pair }

type Pairing =
    { Number: int
      Player1: string * Score
      Player2: string * Score }
    member this.IsScored =
        not (
            (snd this.Player1).Primary = 0
            && (snd this.Player2).Primary = 0
        )

    member this.Score result =
        { this with
            Player1 = fst this.Player1, fst result
            Player2 = fst this.Player2, snd result }
