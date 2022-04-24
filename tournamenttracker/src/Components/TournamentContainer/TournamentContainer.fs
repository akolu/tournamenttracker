module App.Components.TournamentContainer

open Feliz
open Fable.FontAwesome
open Fable.FontAwesome.Free
open App.Components.Tabs
open App.Components.Settings
open App.Components.Round
open Feliz.Bulma

Fable.Core.JsInterop.importSideEffects "./TournamentContainer.scss"

[<ReactComponent>]
let TournamentContainer () =

    let changeTab (tab: int) =
        System.Console.WriteLine(sprintf "Tab %d selected" tab)

    Html.div [
        prop.className "TournamentContainer__div__root"
        prop.children [
            Tabs
                {| onTabChanged = changeTab
                   items =
                    [ Bulma.icon [
                          Html.i [
                              prop.className "fa fa-screwdriver-wrench"
                          ]
                      ]
                      Html.span "1"
                      Html.span "2" ] |}
            Settings()
        ]
    ]
