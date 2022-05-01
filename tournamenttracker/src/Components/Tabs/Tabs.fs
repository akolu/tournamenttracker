module Components.Tabs

open Feliz
open Feliz.Bulma
open Tournament.Round

Fable.Core.JsInterop.importSideEffects "./Tabs.scss"

[<ReactComponent>]
let Tabs
    (props: {| onTabChanged: int -> unit
               selected: int
               rounds: Round list |})
    =

    let tabs =
        [ Bulma.icon [
              Html.i [
                  prop.className "fa fa-screwdriver-wrench"
              ]
          ] ]
        @ (props.rounds
           |> List.map (fun r -> (Html.span r.Number)))

    let isDisabled tab =
        let firstOngoing =
            props.rounds
            |> List.tryFind (fun r -> r.Status = Pregame)

        match firstOngoing with
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
                            prop.className (
                                if (=) props.selected i then
                                    "active"
                                else
                                    ""
                            )
                            prop.children [
                                Html.button [
                                    prop.onClick (fun _ -> props.onTabChanged (i))
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
