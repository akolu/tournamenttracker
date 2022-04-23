module App.Components.TournamentContainer

open Feliz
open App.Components.Tabs
open App.Components.Round

Fable.Core.JsInterop.importSideEffects "./TournamentContainer.scss"

[<ReactComponent>]
let TournamentContainer () =

    let changeTab (tab: int) =
        System.Console.WriteLine(sprintf "Tab %d selected" tab)

    Html.div [
        prop.className "TournamentContainer__div__root"
        prop.children [
            Tabs
                { onTabChanged = changeTab
                  items =
                    [ Html.span "Settings"
                      Html.span "1"
                      Html.span "2" ] }
            Round()
        ]
    ]
