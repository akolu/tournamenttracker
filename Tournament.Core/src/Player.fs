module Tournament.Player

type Player =
    { Name: string
      Rating: int
      BonusScore: int }

    static member From name =
        { Name = name
          Rating = 0
          BonusScore = 0 }
