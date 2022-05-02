module State

open Tournament.Tournament
open Tournament.Utils
open Tournament.PairingGenerator
open Elmish

type State = { Tournament: Tournament }

let state () =
    { Tournament = { Rounds = []; Players = [] } }, Cmd.none

type TournamentSettings =
    { Rounds: int
      Players: (string * int) list }

type Action =
    | CreateTournament of TournamentSettings
    | StartRound
    | FinishRound
    | Score of {| nr: int; result: int * int |}

let createTournament settings =
    createTournament settings.Rounds
    >>= addPlayers (settings.Players |> List.map (fun p -> fst p))
    >>= pair Shuffle
    |> unwrap

let nextRound tournament =
    tournament
    |> finishRound
    >>= (fun t ->
        match t.CurrentRound with
        | Some _ -> pair Swiss t
        | None -> Ok t)
    |> unwrap

let update (msg: Action) (state: State) =
    match msg with
    | CreateTournament settings -> { state with Tournament = createTournament settings }, Cmd.none
    | StartRound -> { state with Tournament = state.Tournament |> startRound |> unwrap }, Cmd.none
    | FinishRound -> { state with Tournament = state.Tournament |> nextRound }, Cmd.none
    | Score p -> { state with Tournament = state.Tournament |> score p.nr p.result |> unwrap }, Cmd.none
