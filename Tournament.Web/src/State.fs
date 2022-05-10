module State

open Tournament.Tournament
open Tournament.Utils
open Tournament.PairingGenerator
open Tournament.Player
open Elmish
open System

type PageModel =
    | Settings of Settings.State.SettingsModel
    | Round of Round.State.RoundModel
    | Results

type Page =
    { Index: int
      Model: PageModel }
    static member settings p =
        match p with
        | Settings s -> s
        | _ -> raise (Exception("Could not coerce Page to Settings"))

    static member round p =
        match p with
        | Round r -> r
        | _ -> raise (Exception("Could not coerce Page to Round"))

type State =
    { Tournament: Tournament
      CurrentPage: Page }

type Action =
    | Score of {| nr: int; result: int * int |}
    | SetActivePage of int
    | SettingsMsg of Settings.State.SettingsMsg
    | RoundMsg of Round.State.RoundMsg

let state () =
    let settings, settingsCmd =
        Settings.State.init
            3
            [ "Aku Ankka", 0
              "Mikki Hiiri", 0
              "Hessu Hopo", 0
              "Pelle Peloton", 0 ]

    { Tournament = { Rounds = []; Players = [] }
      CurrentPage = { Index = 0; Model = Settings settings } },
    Cmd.map SettingsMsg settingsCmd

let private getActivePage i t =
    match i with
    | 0 ->
        { Index = 0
          Model = Settings(fst (Settings.State.init t.Rounds.Length (List.map (fun p -> p.Name, p.Rating) t.Players))) }
    | num when num > t.Rounds.Length -> { Index = num; Model = Results }
    | num ->
        { Index = num
          Model = Round(fst (Round.State.init num t)) }

let createTournament (settings: Settings.State.SettingsModel) state =
    Tournament.Create(settings.Rounds, (settings.Players |> List.map (fun p -> fst p)))
    >>= pair Shuffle
    |> unwrap
    |> (fun t ->
        { state with
            Tournament = t
            CurrentPage = getActivePage (state.CurrentPage.Index) t })

let private updateTournament fn state =
    state.Tournament
    |> fn
    |> unwrap
    |> (fun t ->
        { state with
            Tournament = t
            CurrentPage = getActivePage (state.CurrentPage.Index) t })

let private nextRound tournament =
    tournament
    |> finishRound
    >>= (fun t ->
        match t.CurrentRound with
        | Some _ -> pair Swiss t
        | None -> Ok t)

let update (msg: Action) (state: State) =
    match msg with
    | Score p -> { state with Tournament = state.Tournament |> score p.nr p.result |> unwrap }, Cmd.none
    | SetActivePage tab -> { state with CurrentPage = getActivePage tab state.Tournament }, Cmd.none
    | SettingsMsg msg' ->
        let res, cmd = Settings.State.update msg' (Page.settings state.CurrentPage.Model)

        match msg' with
        | Settings.State.Confirm ->
            createTournament res state,
            Cmd.batch [
                Cmd.map SettingsMsg cmd
                Cmd.ofMsg (SetActivePage(state.CurrentPage.Index + 1))
            ]
        | _ -> { state with CurrentPage = { state.CurrentPage with Model = Settings res } }, Cmd.map SettingsMsg cmd
    | RoundMsg msg' ->
        let res, cmd = Round.State.update msg' (Page.round state.CurrentPage.Model)

        match msg' with
        | Round.State.StartRound -> updateTournament startRound state, Cmd.map RoundMsg cmd
        | Round.State.FinishRound ->
            updateTournament nextRound state,
            Cmd.batch [
                Cmd.map RoundMsg cmd
                Cmd.ofMsg (SetActivePage(state.CurrentPage.Index + 1))
            ]
        | Round.State.ConfirmScore p ->
            updateTournament (score p.Number (p.Player1Score, p.Player2Score)) state, Cmd.map RoundMsg cmd
        | _ -> { state with CurrentPage = { state.CurrentPage with Model = Round res } }, Cmd.map RoundMsg cmd
