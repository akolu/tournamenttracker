module State

open Tournament.Tournament
open Tournament.Utils
open Tournament.PairingGenerator
open Tournament.Player
open Elmish
open System

type PageModels =
    { Settings: Settings.State.SettingsModel
      Round: Round.State.RoundModel
      Results: Results.State.ResultsModel }

type State =
    { Tournament: Tournament
      PageModels: PageModels
      CurrentTab: int }

type Action =
    | Score of {| nr: int; result: int * int |}
    | SetActivePage of int
    | SettingsMsg of Settings.State.SettingsMsg
    | RoundMsg of Round.State.RoundMsg
    | ResultsMsg of Results.State.ResultsMsg

let getPageModels settings (t: Tournament) =
    let rnd =
        match t.CurrentRound with
        | Some r -> r
        | None -> Tournament.Round.Round.Empty

    { Settings = settings
      Results = Results.State.init t
      Round = Round.State.init rnd.Number t }

let updateStateModels state t =
    { state with
        Tournament = t
        PageModels = getPageModels state.PageModels.Settings t }

let state () =
    let settings =
        Settings.State.init
            3
            [ "Aku Ankka", 0
              "Mikki Hiiri", 0
              "Hessu Hopo", 0
              "Pelle Peloton", 0 ]

    { Tournament = Tournament.Empty
      PageModels = getPageModels settings Tournament.Empty
      CurrentTab = 0 },
    Cmd.none

let setActivePage i state =
    match i with
    | 0 -> { state with CurrentTab = 0 }
    | num when num > state.Tournament.Rounds.Length -> { state with CurrentTab = num }
    | num ->
        { state with
            CurrentTab = num
            PageModels = { state.PageModels with Round = Round.State.init num state.Tournament } }

let createTournament (settings: Settings.State.SettingsModel) state =
    Tournament.Create(settings.Rounds, (settings.Players |> List.map (fun p -> fst p)))
    >>= pair Shuffle
    |> unwrap
    |> (fun t -> updateStateModels state t)

let private updateRound fn state =
    state.Tournament
    |> fn
    |> unwrap
    |> (fun t -> updateStateModels state t)

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
    | SetActivePage tab -> setActivePage tab state, Cmd.none
    | SettingsMsg msg' ->
        let res, cmd = Settings.State.update msg' state.PageModels.Settings

        match msg' with
        | Settings.State.Confirm ->
            createTournament res state,
            Cmd.batch [
                Cmd.map SettingsMsg cmd
                Cmd.ofMsg (SetActivePage(state.CurrentTab + 1))
            ]
        | _ -> { state with PageModels = { state.PageModels with Settings = res } }, Cmd.map SettingsMsg cmd
    | RoundMsg msg' ->
        let res, cmd = Round.State.update msg' state.PageModels.Round

        match msg' with
        | Round.State.StartRound -> updateRound startRound state, Cmd.map RoundMsg cmd
        | Round.State.FinishRound ->
            updateRound nextRound state,
            Cmd.batch [
                Cmd.map RoundMsg cmd
                Cmd.ofMsg (SetActivePage(state.CurrentTab + 1))
            ]
        | Round.State.ConfirmScore p ->
            updateRound (score p.Number (p.Player1Score, p.Player2Score)) state, Cmd.map RoundMsg cmd
        | _ -> { state with PageModels = { state.PageModels with Round = res } }, Cmd.map RoundMsg cmd
    | ResultsMsg msg' ->
        let res, cmd = Results.State.update msg' state.PageModels.Results

        match msg' with
        | Results.State.ConfirmBonus p ->
            let t = state.Tournament |> (bonus p) |> unwrap

            { state with
                Tournament = t
                PageModels = { state.PageModels with Results = res } },
            Cmd.map ResultsMsg cmd
        | _ -> { state with PageModels = { state.PageModels with Results = res } }, Cmd.map ResultsMsg cmd
