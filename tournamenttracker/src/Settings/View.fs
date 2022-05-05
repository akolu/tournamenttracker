module Settings.View

open Feliz
open Feliz.Bulma
open Fable.FontAwesome
open State
open Components.IconButton

Fable.Core.JsInterop.importSideEffects "./Styles.scss"

let view (state: SettingsModel) (dispatch: SettingsMsg -> unit) =
    Html.div [
        prop.className "Settings__div--root"
        prop.children [
            Bulma.columns [
                prop.children [
                    Bulma.column [
                        prop.children [
                            Bulma.Divider.divider "Configuration"
                            Html.div [
                                prop.className "Settings__div--field"
                                prop.children [
                                    Bulma.label [ prop.text "Name" ]
                                    Bulma.input.text [
                                        input.isRounded
                                        input.isSmall
                                        prop.defaultValue "Tournament name"
                                    ]
                                ]
                            ]
                            Html.div [
                                prop.className "Settings__div--field"
                                prop.children [
                                    Bulma.label "Rounds"
                                    Html.div [
                                        prop.className "Settings__NumberSelector--wrapper"
                                        prop.children [
                                            IconButton(Fa.Solid.Minus, (fun _ -> dispatch RemoveRounds))
                                            Html.div state.Rounds
                                            IconButton(Fa.Solid.Plus, (fun _ -> dispatch AddRounds))
                                        ]
                                    ]
                                ]
                            ]
                            Html.div [
                                prop.className "Settings__div--field"
                                prop.children [
                                    Bulma.label "Players"
                                    Html.div [
                                        prop.className "Settings__NumberSelector--wrapper"
                                        prop.children [
                                            IconButton(Fa.Solid.Minus, (fun _ -> dispatch RemovePlayers))
                                            Html.div state.Players.Length
                                            IconButton(Fa.Solid.Plus, (fun _ -> dispatch AddPlayers))
                                        ]
                                    ]
                                ]
                            ]
                            Bulma.button.button [
                                button.isSmall
                                button.isRounded
                                prop.onClick (fun _ -> dispatch (Confirm state))
                                prop.className "Settings__button--save"
                                prop.children [
                                    Bulma.icon (Fa.i [ Fa.Solid.Save ] [])
                                    Html.b "Save"
                                ]
                            ]
                        ]
                    ]
                    Bulma.column [
                        prop.children [
                            Bulma.Divider.divider "Players"
                            Html.div (
                                state.Players
                                |> List.mapi (fun i p ->
                                    Html.div [
                                        prop.className "Settings__Players--inputwrapper"
                                        prop.children [
                                            Bulma.input.text [
                                                input.isRounded
                                                input.isSmall
                                                prop.onChange (fun (ev: string) -> dispatch (EditPlayerName(i, ev)))
                                                prop.placeholder "Player name"
                                                prop.defaultValue (fst p)
                                            ]
                                            // Bulma.input.text [
                                            //     input.isRounded
                                            //     input.isSmall
                                            //     prop.placeholder "Rating"
                                            // ]
                                            ]
                                    ])
                            )
                        ]
                    ]
                    // Bulma.column [
                    //     column.isOneThird
                    //     prop.children [
                    //         Bulma.Divider.divider "Info"
                    //     ]
                    // ]
                    ]
            ]
        ]
    ]
