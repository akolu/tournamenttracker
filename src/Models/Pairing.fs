namespace Tournament

type Pairing =
    { Number: int
      Player1: string
      Player2: string
      Player1Score: int
      Player2Score: int }

module Pairing =
    let private allocateToPairings (players: (string * int) list) : Pairing list =
        let makePairing number p1 p2 =
            { Number = number
              Player1 = p1
              Player2 = p2
              Player1Score = 0
              Player2Score = 0 }

        players
        |> List.chunkBySize 2
        |> List.fold
            (fun acc chunk ->
                acc
                @ [ makePairing (acc.Length) (fst chunk.[0]) (fst chunk.[1]) ])
            []

    let internal addPairings (pairingFunc: (string * int) list -> (string * int) list) playersWithScores =
        let playerList = pairingFunc (playersWithScores |> Map.toList)
        allocateToPairings playerList
