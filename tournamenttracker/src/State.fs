module App.State

open Tournament.Tournament
open Tournament.Utils
open Elmish

type State = { Tournament: Tournament }

let state () =
    { Tournament = { Rounds = []; Players = [] } }, Cmd.none

type TournamentSettings =
    { Rounds: int
      Players: (string * int) list }

type Action =
    | CreateTournament of TournamentSettings
    | MakePairings of Tournament.PairingGenerator.PairingAlgorithm

let createTournament settings =
    createTournament settings.Rounds
    >>= addPlayers (settings.Players |> List.map (fun p -> fst p))
    |> unwrap

let update (msg: Action) (state: State) =
    match msg with
    | CreateTournament settings -> { state with Tournament = createTournament settings }, Cmd.none
    | MakePairings alg -> { state with Tournament = pair alg state.Tournament |> unwrap }, Cmd.none
