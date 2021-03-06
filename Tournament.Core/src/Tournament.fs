module Tournament.Tournament

open Round
open Utils
open PairingGenerator
open Pairing
open Player

type Tournament =
    { Rounds: Round list
      Players: Player list }
    member this.CurrentRound =
        this.Rounds
        |> List.tryFind (fun r -> r.Status <> Finished)

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
                   |> List.fold
                       (fun pairs pairing ->
                           pairs
                           @ [ (fst pairing.Player1), (fst pairing.Player2) ])
                       []))
            []

    member this.Pairings: List<Pairing> =
        match this.CurrentRound with
        | Some rnd -> rnd.Pairings
        | None -> (List.rev this.Rounds).Head.Pairings

    member this.Standings rnd =
        if (this.Rounds.IsEmpty) then
            []
        elif this.Rounds.Head.Pairings.IsEmpty then
            this.Players
            |> List.map (fun p -> p.Name, Score.Empty)
        else
            this.Rounds
            |> List.take rnd
            |> List.collect ((fun r -> r.Standings))
            |> List.groupBy (fun p -> fst p)
            |> List.map (fun (player, score) ->
                player,
                { Primary = score |> List.sumBy (fun (_, s) -> s.Primary)
                  Secondary = score |> List.sumBy (fun (_, s) -> s.Secondary) })
            |> List.sortBy (fun (p, score) -> -score.Primary, -score.Secondary, p)

    member this.Standings() = this.Standings this.Rounds.Length

    member this.Finished =
        this.Rounds
        |> Seq.forall (fun r -> r.Status = Finished)

    static member Create(rounds, players: string list) =
        let defaultRound index =
            { Number = (+) index 1
              Pairings = []
              Start = None
              Finish = None }

        if rounds = 0 then
            Error "Tournament should have at least one round"
        elif players.Length <> (Set.ofList players).Count then
            Error "All player names must be unique"
        elif players |> List.exists (fun p -> p.Trim() = "") then
            Error "Empty name is not allowed"
        else
            Ok
                { Rounds = (List.init rounds defaultRound)
                  Players = players |> List.map (fun p -> Player.From p) }

    static member Create rounds = Tournament.Create(rounds, [])

    static member Empty = { Rounds = []; Players = [] }


let rec addPlayers (players: Player list) (tournament: Tournament) =
    if players.IsEmpty then
        Ok tournament
    elif players
         |> List.exists (fun p -> p.Name.Trim() = "") then
        Error "Players with empty name are not allowed"
    else
        match Seq.tryFind (fun player -> List.exists (fun p -> p.Name = player.Name) tournament.Players) players with
        | Some duplicate -> Error(sprintf "Player %s already exists" duplicate.Name)
        | None ->
            { tournament with Players = List.sort (tournament.Players @ [ players.Head ]) }
            |> addPlayers (players.Tail)

let startRound (tournament: Tournament) = tournament.ModifyCurrentRound start

let finishRound (tournament: Tournament) = tournament.ModifyCurrentRound finish

let pair alg (tournament: Tournament) =
    let pairingFunc =
        match alg with
        | Swiss -> swiss tournament.MatchHistory
        | Shuffle -> shuffle

    tournament.ModifyCurrentRound(createPairings pairingFunc (tournament.Standings()))

let score number result (tournament: Tournament) =
    tournament.ModifyCurrentRound(score number result)

let swap player1 player2 (tournament: Tournament) =
    tournament.ModifyCurrentRound(swapPlayers player1 player2)

let bonus (player, score) tournament =
    match List.tryFind (fun p -> p.Name = player) tournament.Players with
    | Some p -> Ok { tournament with Players = (replace ((=) p) { p with BonusScore = score }) tournament.Players }
    | None -> Error(sprintf "Player %s not found" player)
