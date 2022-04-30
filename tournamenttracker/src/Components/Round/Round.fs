module App.Components.Round

open Feliz
open Feliz.Bulma
open Fable.FontAwesome
open App.Context
open App.State
open Tournament.Round
open Tournament.Pairing
open Tournament.PairingGenerator

Fable.Core.JsInterop.importSideEffects "./Round.scss"

[<ReactComponent>]
let Pairing (pairing: Pairing) =
    Html.span [
        prop.children [
            Html.div (sprintf "Table %d: %s vs %s" pairing.Number pairing.Player1 pairing.Player2)
        ]
    ]

[<ReactComponent>]
let Round (round: Round) =

    let (state, dispatch) = React.useContext (tournamentContext)

    Html.div [
        prop.className "Round__div--root"
        prop.children [
            Bulma.columns [
                prop.children [
                    Bulma.column [
                        column.isTwoThirds
                        prop.children [
                            Bulma.button.button [
                                button.isSmall
                                button.isRounded
                                prop.onClick (fun _ -> dispatch (MakePairings Swiss))
                                prop.className "Settings__button--save"
                                prop.children [
                                    Bulma.icon (Fa.i [ Fa.Solid.Save ] [])
                                    Html.b "Pair"
                                ]
                            ]
                            Html.div (round.Pairings |> List.map (fun p -> Pairing(p)))
                        ]
                    ]
                    Bulma.column [
                        column.isOneThird
                        prop.children [
                            Bulma.Divider.divider "Standings"
                            Html.div (
                                round.Standings
                                |> Map.toList
                                |> List.map (fun s -> Html.div ((fst s) + " " + (snd s).ToString()))
                            )
                        ]
                    ]
                ]
            ]
        ]
    ]
