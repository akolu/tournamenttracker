namespace Tournament

open Round

type Tournament =
    { Rounds: List<Round>
      Players: List<string> }
    member internal this.CurrentRound =
        let ongoing = List.tryFind (fun rnd -> rnd.Status <> Finished) this.Rounds

        match ongoing with
        | Some rnd -> Ok rnd
        | None -> Error "Tournament already finished"

    member this.Pairings: List<Pairing> =
        match this.CurrentRound with
        | Ok rnd -> rnd.Pairings
        | Error _ -> (List.rev this.Rounds).Head.Pairings

    member this.Standings =
        let mergeMaps map1 map2 =
            Map.fold
                (fun state key value ->
                    match Map.tryFind key state with
                    | Some v -> Map.add key (value + v) state
                    | None -> Map.add key value state)
                map1
                map2

        let players =
            this.Players
            |> List.map (fun p -> (p, 0))
            |> Map.ofList

        this.Rounds
        |> List.map (fun r -> r.Standings)
        |> List.fold (fun acc scores -> mergeMaps acc scores) players
        |> Map.toList
        |> List.sortBy (fun (_, score) -> -score)

    member this.MatchHistory =
        this.Rounds
        |> List.fold
            (fun acc rnd ->
                acc
                @ (rnd.Pairings
                   |> List.fold (fun pairs pairing -> pairs @ [ pairing.Player1, pairing.Player2 ]) []))
            []

module Tournament =
    let (>>=) x f = Result.bind f x

    let private replace fn item list =
        list
        |> List.map (fun i -> if fn i then item else i)

    let createTournament rounds =
        let defaultRound index =
            { Number = (+) index 1
              Pairings = []
              Status = Pregame }

        match rounds with
        | n when n > 0 ->
            Ok
                { Rounds = (List.init rounds defaultRound)
                  Players = [] }
        | _ -> Error "Tournament should have at least one round"

    let private modifyCurrentRound (modified: Result<Round, string>) (tournament: Tournament) =
        match (tournament.CurrentRound, modified) with
        | (Ok rnd, Ok modified) -> Ok { tournament with Rounds = (replace ((=) rnd) modified) tournament.Rounds }
        | (Error err, _) -> Error err
        | (_, Error err) -> Error err

    let private addPlayer player tournament =
        let existing =
            tournament.Players
            |> List.tryFind (fun p -> (=) player p)

        match existing with
        | Some _ -> Error "Player with that name already exists"
        | None -> Ok { tournament with Players = player :: tournament.Players |> List.sort }

    let rec addPlayers players tournament =
        match players with
        | [] -> Ok tournament
        | [ x ] -> addPlayer x tournament
        | x :: rest -> addPlayer x tournament >>= addPlayers rest

    let startRound (tournament: Tournament) =
        match tournament.CurrentRound with
        | Ok rnd -> modifyCurrentRound (start rnd) tournament
        | Error err -> Error err

    let finishRound (tournament: Tournament) =
        match tournament.CurrentRound with
        | Ok rnd -> modifyCurrentRound (finish rnd) tournament
        | Error err -> Error err

    let pair alg (tournament: Tournament) =
        let pairingFunc =
            match alg with
            | PairingGenerator.Swiss -> PairingGenerator.swiss tournament.MatchHistory
            | PairingGenerator.Shuffle -> PairingGenerator.shuffle

        match tournament.CurrentRound with
        | Ok rnd when rnd.Status = Pregame ->
            let pairings = (createPairings pairingFunc tournament.Standings)
            modifyCurrentRound (createPairings pairingFunc tournament.Standings rnd) tournament
        | Ok rnd -> Error(sprintf "Unable to pair: round %i already started" rnd.Number)
        | Error err -> Error err

    let score number result (tournament: Tournament) =
        match tournament.CurrentRound with
        | Ok rnd when rnd.Status = Ongoing -> modifyCurrentRound (score number result rnd) tournament
        | Ok rnd -> Error(sprintf "Unable to score: round %i not started" rnd.Number)
        | Error err -> Error err

    let swap player1 player2 (tournament: Tournament) =
        match tournament.CurrentRound with
        | Ok rnd -> modifyCurrentRound (swapPlayers player1 player2 rnd) tournament
        | Error err -> Error err
