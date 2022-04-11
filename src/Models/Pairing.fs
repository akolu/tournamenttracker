namespace Tournament

type Pairing =
    { Number: int
      Player1: string
      Player2: string
      Player1Score: int
      Player2Score: int }

module Pairing =
    // let private allocateToPairings (players: ((string * int) * (string * int)) list)) : Pairing list =
    //     let makePairing number (p1, p2) =
    //         { Number = number
    //           Player1 = p1
    //           Player2 = p2
    //           Player1Score = 0
    //           Player2Score = 0 }
    //     players |> List.mapi (fun index (p1, p2) -> makePairing index pairing )

    let private playerPairsToPairings (pairs: ((string * int) * (string * int)) list) : Pairing list =
        pairs
        |> List.mapi (fun i ((p1Name, _), (p2Name, _)) ->
            { Number = i
              Player1 = p1Name
              Player2 = p2Name
              Player1Score = 0
              Player2Score = 0 })

    let internal addPairings
        (pairingFunc: (string * int) list -> ((string * int) * (string * int)) list)
        (standings: Map<string, int>)
        : List<Pairing> =
        (pairingFunc (standings |> Map.toList))
        |> playerPairsToPairings
