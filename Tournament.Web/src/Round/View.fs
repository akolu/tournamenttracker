module Round.View

open Feliz
open Feliz.Bulma
open Fable.FontAwesome
open Tournament.Round
open Tournament.Pairing
open Components.Standings
open Browser.Types
open Round.State

Fable.Core.JsInterop.importSideEffects "./Styles.scss"

let context = React.createContext ()

[<ReactComponent>]
let private Pairing (pairing: Pairing) =

    let (state, dispatch) = React.useContext (context)

    let mutable p1Ref = React.useRef<Browser.Types.Element> (null)

    let editing =
        match state.Form with
        | Some f when f.Number = pairing.Number -> true
        | _ -> false

    React.useEffect (
        (fun _ ->
            if editing then
                (unbox<HTMLInputElement> p1Ref.current).select ()),
        [| box editing |]
    )

    let score (num: int) (inputProps: IReactProperty list) =
        if editing then
            Html.input (
                [ prop.type' "number"
                  prop.onKeyDown (fun e ->
                      match e.key, state.Form with
                      | "Enter", Some p -> dispatch (ConfirmScore p)
                      | "Escape", _ -> dispatch (Edit None)
                      | _ -> ())
                  prop.inputMode.numeric
                  prop.pattern (System.Text.RegularExpressions.Regex "[0-9]*")
                  input.isSmall
                  prop.defaultValue num ]
                @ inputProps
            )
        elif pairing.IsScored then
            Html.b num
        else
            Html.span "-"

    Html.div [
        prop.classes [
            if editing then
                "Pairing__div--clickable"
            else
                ""
        ]
        prop.onClick (fun _ -> dispatch (Edit(Some pairing)))
        prop.children [
            Html.span pairing.Number
            Html.span pairing.Player1.Name
            Html.span [
                prop.children (
                    score
                        pairing.Player1Score
                        [ prop.ref (fun node -> p1Ref.current <- node)
                          prop.onChange (fun num -> dispatch (SetPlayer1Score num)) ]
                )
            ]
            Html.span pairing.Player2.Name
            Html.span [
                prop.children (score pairing.Player2Score [ prop.onChange (fun num -> dispatch (SetPlayer2Score num)) ])
            ]
        ]
    ]

let Round (state: RoundModel) (dispatch: RoundMsg -> unit) =
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
                            Html.div [
                                prop.classes [
                                    "Round__pairings_grid-wrapper"
                                    match state.Round.Status with
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
                                    @ (state.Round.Pairings
                                       |> List.map (fun p ->
                                           React.contextProvider (context, (state, dispatch), Pairing(p))))
                                )
                            ]
                        ]
                    ]
                    Bulma.column [
                        column.isOneThird
                        prop.children [
                            Bulma.Divider.divider "Standings"
                            Standings([ state.Round ], state.StandingsAcc)
                        ]
                    ]
                ]
            ]
            Html.span (
                match state.Round.Status, state.Form with
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
                | (Ongoing, Some p) ->
                    [ Bulma.button.button [
                          button.isSmall
                          button.isRounded
                          prop.onClick (fun _ -> dispatch (Edit None))
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
                          prop.onClick (fun _ -> dispatch (ConfirmScore p))
                          prop.children [
                              Bulma.icon (Fa.i [ Fa.Solid.Check ] [])
                              Html.b "Confirm"
                          ]
                      ] ]
                | (Ongoing, None) ->
                    [ Bulma.button.button [
                          button.isSmall
                          button.isRounded
                          prop.disabled (
                              not (
                                  state.Round.Pairings
                                  |> Seq.forall (fun p -> p.IsScored)
                              )
                          )
                          prop.onClick (fun _ -> dispatch FinishRound)
                          prop.className "Round__button--finish"
                          prop.children [
                              Bulma.icon (Fa.i [ Fa.Solid.FlagCheckered ] [])
                              Html.b "Finish round"
                          ]
                      ] ]
                | (Finished, _) -> []
            )
        ]
    ]
