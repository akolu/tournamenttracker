module Components.TournamentContainer

open Feliz
open State
open Components.Tabs
open Components.Results
open Context

Fable.Core.JsInterop.importSideEffects "./TournamentContainer.scss"

[<ReactComponent>]
let TournamentContainer () =

    let (state, dispatch) = React.useContext (tournamentContext)

    let page =
        match state.CurrentPage.Model with
        | Settings s -> Settings.View.Settings s (SettingsMsg >> dispatch)
        | Round rnd -> Round.View.Round rnd (RoundMsg >> dispatch)
        | _ -> Results()

    Html.div [
        prop.className "TournamentContainer__div--root"
        prop.children [
            Tabs
                {| onTabChanged = (fun tab -> dispatch (SetActivePage tab))
                   selected = state.CurrentPage.Index
                   rounds = state.Tournament.Rounds |}
            Html.div [
                prop.className "TournamentContainer__div--content"
                prop.children page
            ]
        ]
    ]
