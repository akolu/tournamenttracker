module Tournament.State

open Elmish
open Tournament.Tournament
open Tournament.Utils
open Tournament.PairingGenerator
open Pairing
open System

type PageModels =
    { Settings: Settings.State.SettingsModel
      Rounds: Map<int, Round.State.RoundModel>
      Results: Results.State.ResultsModel }

type TournamentModel =
    { Tournament: Tournament
      PageModels: PageModels
      CurrentTab: int }

type TournamentMsg =
    | SetActivePage of int
    | SettingsMsg of Settings.State.SettingsMsg
    | RoundMsg of Round.State.RoundMsg
    | ResultsMsg of Results.State.ResultsMsg

let getPageModels (t: Tournament) =
    { Settings = Settings.State.init t
      Results = Results.State.init t
      Rounds =
        Map.ofList (
            t.Rounds
            |> List.map (fun r -> r.Number, Round.State.init r.Number t)
        ) }

let updateStateModels state (t: Tournament) =
    { state with
        Tournament = t
        PageModels = getPageModels t }

let init () =
    { Tournament = Tournament.Empty
      PageModels =
        { Settings = Settings.State.init Tournament.Empty
          Rounds = Map.empty
          Results = Results.State.init Tournament.Empty }
      CurrentTab = 0 }

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

let getStrengthOfSchedule (model: Round.State.RoundModel) (pairing: Pairing) =
    match model.Round.Start,
          model.Round.Pairings
          |> List.tryFind (fun p -> (=) p.Number pairing.Number)
        with
    | _, Some p when p.IsScored -> pairing // pairing was already scored before -> do not modify end time
    | Some t, Some _ ->
        let seconds =
            ((-) (DateTimeOffset(t).ToUnixTimeSeconds()) (DateTimeOffset(DateTime.Now).ToUnixTimeSeconds())
             |> int)
        // secondary score is negative because secondary score is ordered as descending and less elapsed time is better
        { pairing with
            Player1 = fst pairing.Player1, { snd pairing.Player1 with Secondary = -seconds }
            Player2 = fst pairing.Player2, { snd pairing.Player2 with Secondary = -seconds } }
    | _ ->
        { pairing with
            Player1 = fst pairing.Player1, { snd pairing.Player1 with Secondary = 0 }
            Player2 = fst pairing.Player2, { snd pairing.Player2 with Secondary = 0 } }

let update msg state =
    match msg with
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
        | Settings.State.Reset -> init (), Cmd.map SettingsMsg cmd
        | _ -> { state with PageModels = { state.PageModels with Settings = res } }, Cmd.map SettingsMsg cmd
    | RoundMsg msg' ->
        match msg' with
        | Round.State.StartRound -> tournamentUpdated startRound state, Cmd.none
        | Round.State.FinishRound -> tournamentUpdated nextRound state, Cmd.ofMsg (SetActivePage(state.CurrentTab + 1))
        | Round.State.ConfirmScore p ->
            tournamentUpdated (score p.Number (snd p.Player1, snd p.Player2)) state, Cmd.none
        | _ -> replaceRound msg' state
    | ResultsMsg msg' ->
        let res, cmd = Results.State.update msg' state.PageModels.Results

        match msg' with
        | Results.State.ConfirmBonus ->
            tournamentUpdated
                (fun t ->
                    (Ok t, state.PageModels.Results.Bonus)
                    ||> Map.fold (fun acc p s -> acc >>= bonus (p, s)))
                state,
            Cmd.none
        | _ -> { state with PageModels = { state.PageModels with Results = res } }, Cmd.map ResultsMsg cmd
