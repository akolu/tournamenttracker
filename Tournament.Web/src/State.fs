module State

open Tournament.Tournament
open Tournament.Utils
open Tournament.PairingGenerator
open Elmish

type PageModels =
    { Settings: Settings.State.SettingsModel
      Rounds: Map<int, Round.State.RoundModel>
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
    { Settings = settings
      Results = Results.State.init t
      Rounds =
        Map.ofList (
            t.Rounds
            |> List.map (fun r -> r.Number, Round.State.init r.Number t)
        ) }

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

let createTournament (settings: Settings.State.SettingsModel) state =
    Tournament.Create(settings.Rounds, (settings.Players |> List.map (fun p -> fst p)))
    >>= pair Shuffle
    |> unwrap
    |> (fun t -> updateStateModels state t)

let private tournamentUpdated fn state =
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

let replaceRound msg state =
    match state.Tournament.CurrentRound with
    | Some rnd ->
        let res, cmd = Round.State.update msg state.PageModels.Rounds.[rnd.Number]

        { state with PageModels = { state.PageModels with Rounds = Map.add rnd.Number res state.PageModels.Rounds } },
        Cmd.map RoundMsg cmd
    | None -> state, Cmd.none

let update (msg: Action) (state: State) =
    match msg with
    | Score p -> { state with Tournament = state.Tournament |> score p.nr p.result |> unwrap }, Cmd.none
    | SetActivePage tab -> { state with CurrentTab = tab }, Cmd.ofMsg (RoundMsg(Round.State.RoundMsg.Edit None))
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
        match msg' with
        | Round.State.StartRound -> tournamentUpdated startRound state, Cmd.none
        | Round.State.FinishRound -> tournamentUpdated nextRound state, Cmd.ofMsg (SetActivePage(state.CurrentTab + 1))
        | Round.State.ConfirmScore p ->
            tournamentUpdated (score p.Number (p.Player1Score, p.Player2Score)) state, Cmd.none
        | _ -> replaceRound msg' state
    | ResultsMsg msg' ->
        let res, cmd = Results.State.update msg' state.PageModels.Results
        { state with PageModels = { state.PageModels with Results = res } }, Cmd.map ResultsMsg cmd
