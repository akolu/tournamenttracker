module Components.Round

open Feliz
open Feliz.Bulma
open Fable.FontAwesome
open Context
open State
open Tournament.Round
open Tournament.Pairing
open Browser.Types

Fable.Core.JsInterop.importSideEffects "./Round.scss"

type PairingAction =
    | Click
    | KeyPress of KeyboardEvent
    | SetPlayer1Score of Event
    | SetPlayer2Score of Event

[<ReactComponent>]
let Pairing
    (props: {| pairing: Pairing
               editing: bool
               handleAction: PairingAction -> unit |})
    =

    let mutable p1Ref = React.useRef<Browser.Types.Element> (null)

    let focusP1Input =
        (fun _ ->
            if props.editing then
                (unbox<HTMLInputElement> p1Ref.current).select ())

    React.useEffect (focusP1Input, [| box props.editing |])

    let score (num: int) (inputProps: IReactProperty list) =
        if props.editing then
            Html.input (
                [ prop.type' "number"
                  prop.onKeyDown (fun e -> props.handleAction (KeyPress e))
                  prop.inputMode.numeric
                  prop.pattern (System.Text.RegularExpressions.Regex "[0-9]*")
                  input.isSmall
                  prop.defaultValue num ]
                @ inputProps
            )
        elif props.pairing.IsScored then
            Html.b num
        else
            Html.span "-"

    Html.div [
        prop.classes [
            if props.editing then
                "Pairing__div--clickable"
            else
                ""
        ]
        prop.onClick (fun _ -> props.handleAction Click)
        prop.children [
            Html.span props.pairing.Number
            Html.span props.pairing.Player1
            Html.span [
                prop.children (
                    score
                        props.pairing.Player1Score
                        [ prop.ref (fun node -> p1Ref.current <- node)
                          prop.onChange (fun num -> (props.handleAction (SetPlayer1Score num))) ]
                )
            ]
            Html.span props.pairing.Player2
            Html.span [
                prop.children (
                    score
                        props.pairing.Player2Score
                        [ prop.onChange (fun num -> props.handleAction (SetPlayer2Score num)) ]
                )
            ]
        ]
    ]

[<ReactComponent>]
let Round (round: Round) =

    let (state, dispatch) = React.useContext (tournamentContext)
    let (editing, setEditing) = React.useState<option<int>> (None)
    let (score, setScore) = React.useState<option<int * (int * int)>> (None)

    let markPairingScored s =
        dispatch (Score {| nr = fst s; result = snd s |})
        setScore None

    React.useEffect (
        (fun _ ->
            match score with
            | Some s -> setEditing (Some(fst s))
            | _ -> setEditing (None)),
        [| box score |]
    )

    let getVal (e: Event) =
        match System.Int32.TryParse (unbox<HTMLInputElement> e.target).value with
        | (true, v) -> v
        | _ -> 0

    let handlePairingAction action p =
        match action, score with
        | (Click, _) when round.Status = Ongoing -> setScore (Some(p.Number, (p.Player1Score, p.Player2Score)))
        | (KeyPress e, Some s) ->
            match e.key with
            | "Enter" -> markPairingScored s
            | "Escape" -> setScore None
            | _ -> ()
        | (SetPlayer1Score e, Some s) -> setScore (Some(fst s, (getVal e, snd (snd s))))
        | (SetPlayer2Score e, Some s) -> setScore (Some(fst s, (fst (snd s), getVal e)))
        | _ -> ()

    let actions =
        match round.Status, score with
        | (Pregame, _) ->
            [ Bulma.button.button [
                  button.isSmall
                  button.isRounded
                  prop.onClick (fun _ -> dispatch StartRound)
                  prop.children [
                      Bulma.icon (Fa.i [ Fa.Solid.Play ] [])
                      Html.b "Start round"
                  ]
              ] ]
        | (Ongoing, Some s) ->
            [ Bulma.button.button [
                  button.isSmall
                  button.isRounded
                  prop.onClick (fun _ -> setScore None)
                  prop.children [
                      Bulma.icon (
                          Html.i [
                              prop.classes [ "fa"; "fa-xmark" ]
                          ]
                      )
                      Html.b "Cancel"
                  ]
              ]
              Bulma.button.button [
                  button.isSmall
                  button.isRounded
                  prop.onClick (fun _ -> markPairingScored s)
                  prop.children [
                      Bulma.icon (Fa.i [ Fa.Solid.Check ] [])
                      Html.b "Confirm"
                  ]
              ] ]
        | (Ongoing, None) ->
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
        | (Finished, _) -> []

    let pairings =
        Html.div [
            prop.classes [
                "Round__pairings_grid-wrapper"
                match round.Status with
                | Pregame -> "Round__div--pregame"
                | Ongoing -> "Round__div--ongoing"
                | _ -> ""
            ]
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
                              editing = ((=) editing (Some p.Number))
                              handleAction = (fun a -> handlePairingAction a p) |}
                       )))
            )
        ]

    Html.div [
        prop.className "Round__root"
        prop.children [
            Bulma.columns [
                prop.children [
                    Bulma.column [
                        prop.className "Round__pairings-root"
                        column.isTwoThirds
                        prop.children [
                            Bulma.Divider.divider "Pairings"
                            pairings
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
            Html.span actions
        ]
    ]
