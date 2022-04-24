module App.Components.Settings

open Feliz
open Feliz.Bulma

Fable.Core.JsInterop.importSideEffects "./Settings.scss"

[<ReactComponent>]
let private IconButton (icon: ReactElement) =
    Bulma.button.button [
        button.isRounded
        button.isSmall
        prop.className "Settings__button__IconButton"
        prop.children [ Bulma.icon icon ]
    ]

[<ReactComponent>]
let Settings () =
    Html.div [
        prop.className "Settings__div__root"
        prop.children [
            Bulma.columns [
                Bulma.column [
                    column.isFull
                    prop.children [
                        Bulma.columns [
                            Bulma.column [
                                column.isOneThird
                                prop.children [
                                    Bulma.Divider.divider "Configuration"
                                    Html.div [
                                        prop.className "Settings__div__field"
                                        prop.children [
                                            Bulma.label [ prop.text "Name" ]
                                            Bulma.input.text [
                                                input.isRounded
                                                input.isSmall
                                                prop.value "Tournament name"
                                            ]
                                        ]
                                    ]
                                    Html.div [
                                        prop.className "Settings__div__field"
                                        prop.children [
                                            Bulma.label "Rounds"
                                            Html.div [
                                                prop.className "Settings__div__rounds"
                                                prop.children [
                                                    IconButton(Html.i [ prop.className "fa fa-minus" ])
                                                    Html.div "1"
                                                    IconButton(Html.i [ prop.className "fa fa-plus" ])
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                            Bulma.column [
                                column.isOneThird
                                prop.children [
                                    Bulma.Divider.divider "Players"
                                    Html.div [
                                        prop.className "Settings__Players--inputwrapper"
                                        prop.children [
                                            Bulma.input.text [
                                                input.isRounded
                                                input.isSmall
                                                prop.placeholder "Enter player name"
                                            ]
                                            IconButton(Html.i [ prop.className "fa fa-plus" ])
                                        ]
                                    ]
                                ]
                            ]
                            Bulma.column [
                                column.isOneThird
                                prop.children [
                                    Bulma.Divider.divider "Info"
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
