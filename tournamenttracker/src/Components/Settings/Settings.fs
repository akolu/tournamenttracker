module App.Components.Settings

open Feliz
open Feliz.Bulma
open Fable.FontAwesome

Fable.Core.JsInterop.importSideEffects "./Settings.scss"

[<ReactComponent>]
let private IconButton (icon: Fa.IconOption, fn: Browser.Types.MouseEvent -> unit) =
    Bulma.button.button [
        button.isRounded
        button.isSmall
        prop.onClick fn
        prop.className "Settings__button__IconButton"
        prop.children [
            Bulma.icon (Fa.i [ icon ] [])
        ]
    ]

type private IconButtonAction =
    | IncRounds
    | DecRounds
    | IncPlayers
    | DecPlayers

[<ReactComponent>]
let Settings () =

    let (rounds, setRounds) = React.useState (1)
    let (players, setPlayers) = React.useState [ ("", 0) ]

    let iconClick action =
        (fun _ ->
            match action with
            | DecRounds when rounds > 1 -> setRounds (rounds - 1)
            | IncRounds when rounds < 9 -> setRounds (rounds + 1)
            | IncPlayers -> setPlayers (players |> List.append [ ("", 0) ])
            | DecPlayers when players.Length > 1 -> setPlayers (players.[.. players.Length - 2])
            | DecPlayers when players.Length = 1 -> setPlayers ([ "", 0 ])
            | _ -> ())

    let playerNameChanged index name =
        setPlayers (
            players
            |> List.mapi (fun i p -> if i = index then (name, snd p) else p)
        )

    let printPlayers (event: Browser.Types.KeyboardEvent) =
        if event.key = "Enter" then
            players
            |> Seq.iter (fun p -> System.Console.WriteLine(fst p + ": " + (snd p).ToString()))

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
                                                prop.defaultValue "Tournament name"
                                            ]
                                        ]
                                    ]
                                    Html.div [
                                        prop.className "Settings__div__field"
                                        prop.children [
                                            Bulma.label "Rounds"
                                            Html.div [
                                                prop.className "Settings__NumberSelector--wrapper"
                                                prop.children [
                                                    IconButton(Fa.Solid.Minus, iconClick DecRounds)
                                                    Html.div rounds
                                                    IconButton(Fa.Solid.Plus, iconClick IncRounds)
                                                ]
                                            ]
                                        ]
                                    ]
                                    Html.div [
                                        prop.className "Settings__div__field"
                                        prop.children [
                                            Bulma.label "Players"
                                            Html.div [
                                                prop.className "Settings__NumberSelector--wrapper"
                                                prop.children [
                                                    IconButton(Fa.Solid.Minus, iconClick DecPlayers)
                                                    Html.div players.Length
                                                    IconButton(Fa.Solid.Plus, iconClick IncPlayers)
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
                                    Html.div (
                                        players
                                        |> List.mapi (fun i p ->
                                            Html.div [
                                                prop.className "Settings__Players--inputwrapper"
                                                prop.children [
                                                    Bulma.input.text [
                                                        input.isRounded
                                                        input.isSmall
                                                        prop.onKeyDown printPlayers
                                                        prop.onChange (fun (ev: string) -> playerNameChanged i ev)
                                                        prop.placeholder "Player name"
                                                    ]
                                                    Bulma.input.text [
                                                        input.isRounded
                                                        input.isSmall
                                                        prop.placeholder "Rating"
                                                    ]
                                                ]
                                            ])
                                    )
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
