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
    | MakePairings of int
    | StartRound
    | FinishRound
    | Score of {| nr: int; result: int * int |}

let createTournament settings =
    createTournament settings.Rounds
    >>= addPlayers (settings.Players |> List.map (fun p -> fst p))
    |> unwrap

let getAlg rnd =
    match rnd with
    | 1 -> Shuffle
    | _ -> Swiss

let update (msg: Action) (state: State) =
    match msg with
    | CreateTournament settings -> { state with Tournament = createTournament settings }, Cmd.none
    | MakePairings rnd -> { state with Tournament = state.Tournament |> pair (getAlg rnd) |> unwrap }, Cmd.none
    | StartRound -> { state with Tournament = state.Tournament |> startRound |> unwrap }, Cmd.none
    | FinishRound -> { state with Tournament = state.Tournament |> finishRound |> unwrap }, Cmd.none
    | Score p -> { state with Tournament = state.Tournament |> score p.nr p.result |> unwrap }, Cmd.none
