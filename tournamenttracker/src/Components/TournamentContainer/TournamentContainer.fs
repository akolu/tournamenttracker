module Components.TournamentContainer

open Feliz
open Components.Tabs
open Components
open State
open Context

Fable.Core.JsInterop.importSideEffects "./TournamentContainer.scss"

[<ReactComponent>]
let TournamentContainer () =

    let (state, dispatch) = React.useContext (tournamentContext)

    let page =
        match state.CurrentPage.Model with
        | Settings s -> Settings.View.root s (SettingsMsg >> dispatch)
        | Round rnd -> Round.View.root rnd (RoundMsg >> dispatch)
        | _ -> Html.span []

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
