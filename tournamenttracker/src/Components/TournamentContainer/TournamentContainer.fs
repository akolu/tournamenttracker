module App.Components.TournamentContainer

open Feliz
open Fable.FontAwesome
open Fable.FontAwesome.Free
open App.Components.Tabs
open App.Components.Settings
open App.Components.Round
open Feliz.Bulma
open App.Context

Fable.Core.JsInterop.importSideEffects "./TournamentContainer.scss"

[<ReactComponent>]
let TournamentContainer () =

    let (state, dispatch) = React.useContext (tournamentContext)

    let changeTab (tab: int) =
        System.Console.WriteLine(sprintf "Tab %d selected" tab)

    let renderTabs =
        [ Bulma.icon [
              Html.i [
                  prop.className "fa fa-screwdriver-wrench"
              ]
          ] ]
        @ (state.Tournament.Rounds
           |> List.map (fun r -> (Html.span r.Number)))

    Html.div [
        prop.className "TournamentContainer__div__root"
        prop.children [
            Tabs
                {| onTabChanged = changeTab
                   items = renderTabs |}
            Settings()
        ]
    ]
