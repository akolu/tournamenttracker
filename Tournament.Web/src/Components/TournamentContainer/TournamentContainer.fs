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
            match state.CurrentPage.Model with
            | Settings s -> Settings.View.Settings(s, (SettingsMsg >> dispatch))
            | Round rnd -> Round.View.Round(rnd, (RoundMsg >> dispatch))
            | Results res -> Results.View.Results(res, (ResultsMsg >> dispatch))

        Html.div [
            prop.className "TournamentContainer__div--root"
            prop.children [
                Components.Tabs(
                    onTabChanged = (fun tab -> dispatch (SetActivePage tab)),
                    selected = state.CurrentPage.Index,
                    rounds = state.Tournament.Rounds
                )
                Html.div [
                    prop.className "TournamentContainer__div--content"
                    prop.children page
                ]
            ]
        ]
