module Tournament.Tournament

open Round
open Utils
open Tournament.Pairing

type Tournament =
    { Rounds: List<Round>
      Players: List<string> }
    member private this.CurrentRound =
        List.tryFind (fun rnd -> rnd.Status <> Finished) this.Rounds

    member internal this.ModifyCurrentRound(fn: Round -> Result<Round, string>) =
        match this.CurrentRound with
        | Some rnd ->
            match fn rnd with
            | Ok modified -> Ok { this with Rounds = (replace ((=) rnd) modified) this.Rounds }
            | Error err -> Error err
        | None -> Error "Tournament already finished"

    member internal this.MatchHistory =
        this.Rounds
        |> List.fold
            (fun acc rnd ->
                acc
                @ (rnd.Pairings
                   |> List.fold (fun pairs pairing -> pairs @ [ pairing.Player1, pairing.Player2 ]) []))
            []

    member this.Pairings: List<Pairing> =
        match this.CurrentRound with
        | Some rnd -> rnd.Pairings
        | None -> (List.rev this.Rounds).Head.Pairings

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

let rec addPlayers players tournament =
    match players with
    | [] -> Ok tournament
    | [ x ] -> addPlayer x tournament
    | x :: rest -> addPlayer x tournament >>= addPlayers rest

let startRound (tournament: Tournament) = tournament.ModifyCurrentRound start

let finishRound (tournament: Tournament) = tournament.ModifyCurrentRound finish

let pair alg (tournament: Tournament) =
    let pairingFunc =
        match alg with
        | PairingGenerator.Swiss -> PairingGenerator.swiss tournament.MatchHistory
        | PairingGenerator.Shuffle -> PairingGenerator.shuffle

    tournament.ModifyCurrentRound(createPairings pairingFunc tournament.Standings)

let score number result (tournament: Tournament) =
    tournament.ModifyCurrentRound(score number result)

let swap player1 player2 (tournament: Tournament) =
    tournament.ModifyCurrentRound(swapPlayers player1 player2)
