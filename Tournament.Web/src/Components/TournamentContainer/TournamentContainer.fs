module Components.TournamentContainer

open Feliz
open State
open Context
open Components.Tabs

Fable.Core.JsInterop.importSideEffects "./TournamentContainer.scss"

type Components() =

    [<ReactComponent>]
    static member TournamentContainer() =
        let (state, dispatch) = React.useContext (tournamentContext)

        let page =
            match state.CurrentTab with
            | 0 -> Settings.View.Settings(state.PageModels.Settings, (SettingsMsg >> dispatch))
            | rnd when rnd > state.Tournament.Rounds.Length ->
                Results.View.Results(state.PageModels.Results, (ResultsMsg >> dispatch))
            | rnd -> Round.View.Round(state.PageModels.Rounds.[rnd], (RoundMsg >> dispatch))

        Html.div [
            prop.className "TournamentContainer__div--root"
            prop.children [
                Components.Tabs(
                    onTabChanged = (fun tab -> dispatch (SetActivePage tab)),
                    selected = state.CurrentTab,
                    rounds = state.Tournament.Rounds
                )
                Html.div [
                    prop.className "TournamentContainer__div--content"
                    prop.children page
                ]
            ]
        ]
