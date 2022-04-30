module App.Components.TournamentContainer

open Feliz
open Fable.FontAwesome
open Fable.FontAwesome.Free
open App.Components.Tabs
open App.Components.Settings
open App.Components.Round
open Feliz.Bulma
open App.State
open App.Context

Fable.Core.JsInterop.importSideEffects "./TournamentContainer.scss"

[<ReactComponent>]
let TournamentContainer () =

    let (state, dispatch) = React.useContext (tournamentContext)
    let (tab, setTab) = React.useState (0)

    let onTournamentCreated settings =
        dispatch (CreateTournament settings)
        setTab (1)

    let getActiveTab =
        match tab with
        | 0 -> Settings(onTournamentCreated)
        | num ->
            Round(
                state.Tournament.Rounds
                |> List.find (fun r -> ((=) r.Number num))
            )

    Html.div [
        prop.className "TournamentContainer__div--root"
        prop.children [
            Tabs
                {| onTabChanged = setTab
                   selected = tab
                   rounds = state.Tournament.Rounds |}
            Html.div [
                prop.className "TournamentContainer__div--content"
                prop.children getActiveTab
            ]
        ]
    ]
