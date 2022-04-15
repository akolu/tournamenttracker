namespace Tournament

open Pairing
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

    let private addPlayer player tournament =
        let existing =
            tournament.Players
            |> List.tryFind (fun p -> (=) player p)

        match existing with
        | Some _ -> Error "Player with that name already exists"
        | None -> Ok { tournament with Players = player :: tournament.Players |> List.sort }

    let rec addPlayers players (tournament: Tournament) =
        match players with
        | [] -> Ok tournament
        | [ x ] -> addPlayer x tournament
        | x :: rest -> addPlayer x tournament >>= addPlayers rest

    let private modifyCurrentRound fn (tournament: Tournament) =
        match tournament.CurrentRound with
        | Ok rnd -> Ok { tournament with Rounds = (tournament.Rounds |> replace ((=) rnd) (fn rnd)) }
        | Error err -> Error err

    let private modifyCurrentRound2 (modified: Result<Round, string>) (tournament: Tournament) =
        match (tournament.CurrentRound, modified) with
        | (Ok rnd, Ok modified) -> Ok { tournament with Rounds = (tournament.Rounds |> replace ((=) rnd) modified) }
        | (Error err, _) -> Error err
        | (_, Error err) -> Error err

    let startRound (tournament: Tournament) =
        match tournament.CurrentRound with
        | Ok rnd -> modifyCurrentRound2 (start rnd) tournament
        | Error err -> Error err

    let finishRound (tournament: Tournament) =
        match tournament.CurrentRound with
        | Ok rnd when rnd.Status = Ongoing -> modifyCurrentRound (fun rnd -> { rnd with Status = Finished }) tournament
        | Ok rnd -> Error(sprintf "Unable to finish round %i: round not started" rnd.Number)
        | Error err -> Error err

    let private pairingFunc alg (tournament: Tournament) =
        match alg with
        | PairingGenerator.Swiss -> PairingGenerator.swiss tournament.MatchHistory
        | PairingGenerator.Shuffle -> PairingGenerator.shuffle

    let pair (alg: PairingGenerator.PairingAlgorithm) (tournament: Tournament) =
        match tournament.CurrentRound with
        | Ok rnd when rnd.Status = Pregame ->
            let pairings = (addPairings (pairingFunc alg tournament) tournament.Standings)
            modifyCurrentRound (fun rnd -> { rnd with Pairings = pairings }) tournament
        | Ok rnd -> Error(sprintf "Unable to pair: round %i already started" rnd.Number)
        | Error err -> Error err

    let score number result (tournament: Tournament) =
        match tournament.CurrentRound with
        | Ok rnd when rnd.Status = Ongoing ->
            match (List.tryFind (fun (p: Pairing) -> number = p.Number) rnd.Pairings) with
            | Some pairing -> modifyCurrentRound (addResults pairing result) tournament
            | None -> Error(sprintf "Match %i not found!" number)
        | Ok rnd -> Error(sprintf "Unable to score: round %i not started" rnd.Number)
        | Error err -> Error err

    let private swapPlayers p1 p2 pairing =
        let tryReplace =
            function
            | p when ((=) p1 p) -> p2
            | p when ((=) p2 p) -> p1
            | p -> p

        { pairing with
            Player1 = (tryReplace pairing.Player1)
            Player2 = (tryReplace pairing.Player2) }

    let private verifyUnscored pairing round =
        match pairing with
        | p when ((=) p.Player1Score 0) && ((=) p.Player2Score 0) -> Ok round
        | _ -> Error "Can't swap players if either player's round has already been scored!"

    let private validatePlayerSwap player round =
        let exists player pairing =
            ((=) player pairing.Player1)
            || ((=) player pairing.Player2)

        match List.tryFind (exists player) round.Pairings with
        | Some pairing -> verifyUnscored pairing round
        | None -> Error(sprintf "Player %s not found" player)

    let swap player1 player2 (tournament: Tournament) =
        tournament.CurrentRound
        >>= validatePlayerSwap player1
        >>= validatePlayerSwap player2
        >>= (fun rnd ->
            let pairings = List.map (swapPlayers player1 player2) rnd.Pairings
            modifyCurrentRound (fun rnd -> { rnd with Pairings = pairings }) tournament)
