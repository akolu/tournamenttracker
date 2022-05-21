module Tournament.Player

open System.Runtime.InteropServices

type Player =
    { Name: string
      Rating: int
      BonusScore: int }

    static member From(name, [<Optional; DefaultParameterValue(0)>] rating: int) =
        { Name = name
          Rating = rating
          BonusScore = 0 }
