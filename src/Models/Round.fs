namespace Tournament

type RoundStatus =
    | Pregame
    | Ongoing
    | Finished

type Round =
    { Number: int
      Pairings: List<Pairing>
      Status: RoundStatus }
    member this.Standings =
        this.Pairings
        |> List.map (fun p ->
            [ (p.Player1, p.Player1Score)
              (p.Player2, p.Player2Score) ])
        |> List.concat
        |> Map.ofList

module internal Round =

    let private replace fn item list =
        list
        |> List.map (fun i -> if fn i then item else i)

    let private modifyPairing (modified: Pairing) (rnd: Round) =
        { rnd with
            Pairings =
                (rnd.Pairings
                 |> replace (fun p -> ((=) p.Number modified.Number)) modified) }

    let private addResults pairing result round =
        round
        |> modifyPairing
            { pairing with
                Player1Score = fst result
                Player2Score = snd result }

    let score number result round =
        match (List.tryFind (fun (p: Pairing) -> number = p.Number) round.Pairings) with
        | Some pairing -> Ok(addResults pairing result round)
        | None -> Error(sprintf "Match %i not found!" number)

    let start (rnd: Round) =
        match rnd.Status with
        | Pregame when not rnd.Pairings.IsEmpty -> Ok { rnd with Status = Ongoing }
        | Pregame -> Error(sprintf "Unable to start round %i: no pairings" rnd.Number)
        | Ongoing -> Error(sprintf "Unable to start round %i: round already started" rnd.Number)
        | Finished -> Error(sprintf "Unable to start round %i: round already finished" rnd.Number)

    let finish (rnd: Round) =
        match rnd.Status with
        | Pregame -> Error(sprintf "Unable to finish round %i: round not started" rnd.Number)
        | Ongoing when Seq.forall (fun (p: Pairing) -> p.IsScored) rnd.Pairings -> Ok { rnd with Status = Finished }
        | Ongoing -> Error(sprintf "Unable to finish round %i: unscored pairings exist" rnd.Number)
        | Finished -> Error(sprintf "Unable to finish round %i: round already finished" rnd.Number)

    let private playerPairsToPairings (pairs: ((string * int) * (string * int)) list) : Pairing list =
        pairs
        |> List.mapi (fun i ((p1Name, _), (p2Name, _)) ->
            { Number = i
              Player1 = p1Name
              Player2 = p2Name
              Player1Score = 0
              Player2Score = 0 })

    let createPairings
        (pairingFunc: (string * int) list -> ((string * int) * (string * int)) list)
        (standings: List<string * int>)
        round
        =
        let playerList =
            match standings with
            | list when (<>) ((%) list.Length 2) 0 -> standings @ [ ("BYE", 0) ]
            | _ -> standings

        Ok { round with Pairings = (pairingFunc playerList) |> playerPairsToPairings }

    let private trySwap p1 p2 pairing =
        let tryReplace =
            function
            | p when ((=) p1 p) -> p2
            | p when ((=) p2 p) -> p1
            | p -> p

        { pairing with
            Player1 = (tryReplace pairing.Player1)
            Player2 = (tryReplace pairing.Player2) }

    let private validatePlayerSwap player round =
        let exists player pairing =
            ((=) player pairing.Player1)
            || ((=) player pairing.Player2)

        match List.tryFind (exists player) round.Pairings with
        | Some pairing when not pairing.IsScored -> Ok true
        | Some _ -> Error "Can't swap players if either player's round has already been scored!"
        | None -> Error(sprintf "Player %s not found" player)

    let swapPlayers player1 player2 round =
        match (round |> validatePlayerSwap player1, round |> validatePlayerSwap player2) with
        | (Ok _, Ok _) -> Ok { round with Pairings = List.map (trySwap player1 player2) round.Pairings }
        | (Error err, _) -> Error err
        | (_, Error err) -> Error err
