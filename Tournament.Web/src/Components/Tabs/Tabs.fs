module Components.Tabs

open Feliz
open Feliz.Bulma
open Tournament.Round
open Fable.FontAwesome

Fable.Core.JsInterop.importSideEffects "./Tabs.scss"

type Components() =
    [<ReactComponent>]
    static member Tabs(onTabChanged, selected: int, (rounds: Round list)) =
        let tabs =
            let settings =
                [ Bulma.icon [
                      Html.i [
                          prop.className "fa fa-screwdriver-wrench"
                      ]
                  ] ]

            if rounds.IsEmpty then
                settings
            else
                settings
                @ (rounds |> List.map (fun r -> (Html.span r.Number)))
                  @ [ Bulma.icon [
                          Fa.i [ Fa.Solid.Trophy ] []
                      ] ]

        let isDisabled tab =
            if tab > rounds.Length then
                false
            else
                match List.tryFind (fun r -> r.Status <> Finished) rounds with
                | Some r when tab > r.Number -> true
                | _ -> false

        Html.div [
            prop.className "Tabs__div--wrapper"
            prop.children [
                Html.ul [
                    prop.children (
                        tabs
                        |> List.mapi (fun i x ->
                            Html.li [
                                prop.className (if (=) selected i then "active" else "")
                                prop.children [
                                    Html.button [
                                        prop.onClick (fun _ -> onTabChanged (i))
                                        prop.disabled (isDisabled i)
                                        prop.children x
                                    ]
                                ]
                            ])
                    )
                ]
                Html.div [
                    prop.className "Tabs__div--fill"
                    prop.children []
                ]
            ]
        ]
