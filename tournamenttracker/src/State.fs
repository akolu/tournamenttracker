module State

open Tournament.Tournament
open Tournament.Utils
open Tournament.PairingGenerator
open Elmish

type State =
    { Tournament: Tournament
      Settings: Components.Settings.SettingsModel }

type Action =
    // | CreateTournament of TournamentSettings
    | StartRound
    | FinishRound
    | Score of {| nr: int; result: int * int |}
    | SettingsMsg of Components.Settings.SettingsMsg

let state () =
    let settings, settingsCmd = Components.Settings.init ()

    { Tournament = { Rounds = []; Players = [] }
      Settings = settings },
    Cmd.batch [
        Cmd.map SettingsMsg settingsCmd
    ]

let createTournament (settings: Components.Settings.SettingsModel) =
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
    // | CreateTournament settings -> { state with Tournament = createTournament settings }, Cmd.none
    | StartRound -> { state with Tournament = state.Tournament |> startRound |> unwrap }, Cmd.none
    | FinishRound -> { state with Tournament = state.Tournament |> nextRound }, Cmd.none
    | Score p -> { state with Tournament = state.Tournament |> score p.nr p.result |> unwrap }, Cmd.none
    | SettingsMsg msg' ->
        match msg' with
        | Components.Settings.SettingsMsg.Confirm model -> { state with Tournament = createTournament model }, Cmd.none
        | _ ->
            let res, cmd = Components.Settings.update msg' state.Settings
            { state with Settings = res }, Cmd.map SettingsMsg cmd
