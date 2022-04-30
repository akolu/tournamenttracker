module App.State

open Tournament.Tournament
open Tournament.Utils
open Elmish

type State = { Count: int; Tournament: Tournament }

let state () =
    { Count = 0
      Tournament = { Rounds = []; Players = [] } },
    Cmd.none

type TournamentSettings =
    { Rounds: int
      Players: (string * int) list }

type Msg =
    | Increment of int
    | Decrement of int
    | CreateTournament of TournamentSettings

let createTournament settings =
    createTournament settings.Rounds
    >>= addPlayers (settings.Players |> List.map (fun p -> fst p))
    |> unwrap

let update (msg: Msg) (state: State) =
    match msg with
    | Increment num -> { state with Count = state.Count + num }, Cmd.none
    | Decrement num -> { state with Count = state.Count - num }, Cmd.none
    | CreateTournament settings -> { state with Tournament = createTournament settings }, Cmd.none
