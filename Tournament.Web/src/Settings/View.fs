module Settings.View

open Feliz
open Feliz.Bulma
open Fable.FontAwesome
open State
open Components.IconButton

Fable.Core.JsInterop.importSideEffects "./Styles.scss"

[<ReactComponent>]
let Settings (state, dispatch) =

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
                                        prop.disabled (not state.Editable)
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
                                            Components.IconButton(
                                                Fa.Solid.Minus,
                                                [ prop.onClick (fun _ -> dispatch RemoveRounds)
                                                  prop.disabled (not state.Editable) ]
                                            )
                                            Html.div state.Rounds
                                            Components.IconButton(
                                                Fa.Solid.Plus,
                                                [ prop.onClick (fun _ -> dispatch AddRounds)
                                                  prop.disabled (not state.Editable) ]
                                            )
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
                                            Components.IconButton(
                                                Fa.Solid.Minus,
                                                [ prop.onClick (fun _ -> dispatch RemovePlayers)
                                                  prop.disabled (not state.Editable) ]
                                            )
                                            Html.div state.Players.Length
                                            Components.IconButton(
                                                Fa.Solid.Plus,
                                                [ prop.onClick (fun _ -> dispatch AddPlayers)
                                                  prop.disabled (not state.Editable) ]
                                            )
                                        ]
                                    ]
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
                                                prop.value (fst p)
                                                prop.disabled (not state.Editable)
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
            match state.Editable with
            | true ->
                Bulma.button.button [
                    button.isSmall
                    button.isRounded
                    prop.onClick (fun _ -> dispatch Confirm)
                    prop.disabled (state.ValidationErrors.Count > 0)
                    prop.classes [ "Settings__button" ]
                    prop.children [
                        Bulma.icon (Fa.i [ Fa.Solid.Plus ] [])
                        Html.b "Create"
                    ]
                ]
            | false ->
                Bulma.button.button [
                    button.isSmall
                    button.isRounded
                    prop.onClick (fun _ ->
                        if (Browser.Dom.window.confirm "Reset tournament? This action cannot be reverted" = true) then
                            dispatch Reset)
                    prop.classes [ "Settings__button" ]
                    prop.children [
                        Bulma.icon (Fa.i [ Fa.Solid.TrashAlt ] [])
                        Html.b "Reset"
                    ]
                ]
        ]
    ]
