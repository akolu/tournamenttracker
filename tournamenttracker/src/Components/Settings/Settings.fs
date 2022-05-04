module Components.Settings

open Feliz
open Feliz.Bulma
open Fable.FontAwesome
open Elmish
// open State

Fable.Core.JsInterop.importSideEffects "./Settings.scss"


type SettingsModel =
    { Rounds: int
      Players: (string * int) list }

type SettingsMsg =
    | AddRounds
    | RemoveRounds
    | AddPlayers
    | RemovePlayers
    | EditPlayerName of (int * string)
    | Confirm of SettingsModel

let init () = { Rounds = 0; Players = [] }, Cmd.none

let update msg model =
    match msg with
    | AddRounds when model.Rounds < 9 -> { model with Rounds = model.Rounds + 1 }, Cmd.none
    | AddRounds -> model, Cmd.none
    | RemoveRounds when model.Rounds > 1 -> { model with Rounds = model.Rounds - 1 }, Cmd.none
    | RemoveRounds -> model, Cmd.none
    | AddPlayers -> { model with Players = model.Players @ [ ("", 0) ] }, Cmd.none
    | RemovePlayers when model.Players.Length > 1 ->
        { model with Players = model.Players.[.. model.Players.Length - 2] }, Cmd.none
    | RemovePlayers -> model, Cmd.none
    | EditPlayerName (index, name) ->
        { model with
            Players =
                model.Players
                |> List.mapi (fun i p -> if i = index then (name, snd p) else p) },
        Cmd.none
    | Confirm _ -> model, Cmd.none

[<ReactComponent>]
let private IconButton (icon: Fa.IconOption, fn: Browser.Types.MouseEvent -> unit) =
    Bulma.button.button [
        button.isRounded
        button.isSmall
        prop.onClick fn
        prop.className "IconButton__button--root"
        prop.children [
            Bulma.icon (Fa.i [ icon ] [])
        ]
    ]

let SettingsView (state: SettingsModel) (dispatch: SettingsMsg -> unit) =
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
