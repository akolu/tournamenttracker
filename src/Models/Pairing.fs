namespace Tournament

type Pairing =
    { Number: int
      Player1: string
      Player2: string
      Player1Score: int
      Player2Score: int }
    member this.IsScored = not (this.Player1Score = 0 && this.Player2Score = 0)

module internal Pairing =
    let private playerPairsToPairings (pairs: ((string * int) * (string * int)) list) : Pairing list =
        pairs
        |> List.mapi (fun i ((p1Name, _), (p2Name, _)) ->
            { Number = i
              Player1 = p1Name
              Player2 = p2Name
              Player1Score = 0
              Player2Score = 0 })

    let addPairings
        (pairingFunc: (string * int) list -> ((string * int) * (string * int)) list)
        (standings: List<string * int>)
        : List<Pairing> =
        let playerList =
            match standings with
            | list when (<>) ((%) list.Length 2) 0 -> standings @ [ ("BYE", 0) ]
            | _ -> standings

        (pairingFunc playerList) |> playerPairsToPairings
