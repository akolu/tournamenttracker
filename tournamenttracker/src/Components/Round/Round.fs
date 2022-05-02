module Components.Round

open Feliz
open Feliz.Bulma
open Fable.FontAwesome
open Context
open State
open Tournament.Round
open Tournament.Pairing
open Tournament.PairingGenerator
open Browser.Types
open Browser.Dom

Fable.Core.JsInterop.importSideEffects "./Round.scss"

[<ReactComponent>]
let Pairing
    (props: {| pairing: Pairing
               editable: bool
               onEdit: Browser.Types.MouseEvent -> unit
               onCancel: unit -> unit
               onConfirm: (int * int) -> unit |})
    =

    let (p1Score, setP1Score) = React.useState (props.pairing.Player1Score)
    let (p2Score, setP2Score) = React.useState (props.pairing.Player2Score)

    let mutable p1Ref = React.useRef<Browser.Types.Element> (null)
    let mutable p2Ref = React.useRef<Browser.Types.Element> (null)

    let handleKeyDown (e: KeyboardEvent) =
        match e.key with
        | "Enter" -> props.onConfirm (p1Score, p2Score)
        | "Escape" -> props.onCancel ()
        | _ -> ()

    let focusP1Input =
        (fun _ ->
            if props.editable then
                (unbox<HTMLInputElement> p1Ref.current).select ())

    React.useEffect (focusP1Input, [| box props.editable |])

    let score (num: int) (ref: Browser.Types.Element -> unit) (onChange: int -> unit) =
        if props.editable then
            Html.input [
                prop.ref ref
                prop.type' "number"
                prop.onChange onChange
                prop.onKeyDown handleKeyDown
                prop.inputMode.numeric
                prop.pattern (System.Text.RegularExpressions.Regex "[0-9]*")
                input.isSmall
                prop.defaultValue num
            ]
        elif props.pairing.IsScored then
            Html.b num
        else
            Html.span "-"

    Html.div [
        prop.onClick (fun e ->
            if not props.editable then
                props.onEdit e
            else
                ())
        prop.children [
            Html.span props.pairing.Number
            Html.span props.pairing.Player1
            Html.span [
                prop.key "foo"
                prop.children (score props.pairing.Player1Score (fun node -> p1Ref.current <- node) setP1Score)
            ]
            Html.span props.pairing.Player2
            Html.span [
                prop.key "bar"
                prop.children (score props.pairing.Player2Score (fun node -> p2Ref.current <- node) setP2Score)
            ]
            Html.span [
                prop.children [
                    // Html.button [
                    //     prop.hidden props.editable
                    //     prop.onClick props.onEdit
                    //     prop.children [
                    //         Html.i [
                    //             prop.classes [
                    //                 "fa-regular"
                    //                 "fa-pen-to-square"
                    //             ]
                    //         ]
                    //     ]
                    // ]
                    Html.button [
                        prop.hidden (not props.editable)
                        prop.onClick (fun _ -> props.onCancel ())
                        prop.children [
                            Html.i [
                                prop.classes [ "fa"; "fa-xmark" ]
                            ]
                        ]
                    ]
                    Html.button [
                        prop.hidden (not props.editable)
                        prop.onClick (fun _ -> props.onConfirm (p1Score, p2Score))
                        prop.children [
                            (Fa.i [ Fa.Solid.Check ] [])
                        ]
                    ]
                ]
            ]
        ]
    ]

[<ReactComponent>]
let Round (round: Round) =

    let (state, dispatch) = React.useContext (tournamentContext)
    let (editablePairing, setEditablePairing) = React.useState<option<Pairing>> (None)

    let markPairingScored table result =
        dispatch (Score {| nr = table; result = result |})

    let actions =
        match round.Status with
        | Pregame ->
            [ Bulma.button.button [
                  button.isSmall
                  button.isRounded
                  prop.onClick (fun _ -> dispatch (MakePairings round.Number))
                  prop.className "Round__button--save"
                  prop.children [
                      Bulma.icon (
                          Html.i [
                              prop.classes [
                                  "fas"
                                  "fa-people-arrows-left-right"
                              ]
                          ]
                      )
                      Html.b "Pair"
                  ]
              ]
              Bulma.button.button [
                  button.isSmall
                  button.isRounded
                  prop.onClick (fun _ -> dispatch StartRound)
                  prop.className "Round__button--start"
                  prop.children [
                      Bulma.icon (Fa.i [ Fa.Solid.Play ] [])
                      Html.b "Start round"
                  ]
              ] ]
        | Ongoing ->
            [ Bulma.button.button [
                  button.isSmall
                  button.isRounded
                  prop.disabled (not (round.Pairings |> Seq.forall (fun p -> p.IsScored)))
                  prop.onClick (fun _ -> dispatch FinishRound)
                  prop.className "Round__button--finish"
                  prop.children [
                      Bulma.icon (Fa.i [ Fa.Solid.FlagCheckered ] [])
                      Html.b "Finish round"
                  ]
              ] ]
        | Finished -> []

    let pairings =
        Html.div [
            prop.className "Round__div--pairings"
            prop.children (
                [ Html.div [
                      prop.children [
                          Html.b "Table"
                          Html.b "Player 1"
                          Html.b "Score"
                          Html.b "Player 2"
                          Html.b "Score"
                      ]
                  ] ]
                @ (round.Pairings
                   |> List.map (fun p ->
                       Pairing(
                           {| pairing = p
                              editable = ((=) editablePairing (Some p))
                              onEdit = (fun _ -> setEditablePairing (Some p))
                              onCancel = (fun _ -> setEditablePairing None)
                              onConfirm = markPairingScored p.Number |}
                       )))
            )
        ]

    Html.div [
        prop.className "Round__div--root"
        prop.children [
            Bulma.columns [
                prop.children [
                    Bulma.column [
                        column.isTwoThirds
                        prop.children (actions @ [ pairings ])
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
