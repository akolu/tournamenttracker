module Round.View

open Feliz
open Feliz.Bulma
open Fable.FontAwesome
open Tournament.Round
open Tournament.Pairing
open Components.Standings
open Browser.Types
open Round.State
open System
open System.Globalization

Fable.Core.JsInterop.importSideEffects "./Styles.scss"

let context = React.createContext ()

let secondary (start: DateTime option) (pairing: Pairing) =
    match start with
    | Some _ when
        pairing.IsScored
        && (snd pairing.Player1).Secondary <> 0
        && (snd pairing.Player2).Secondary <> 0
        ->
        pairing // pairing was already scored before -> do not modify end time
    | Some start when pairing.IsScored ->
        let seconds =
            ((-) (DateTimeOffset(start).ToUnixTimeSeconds()) (DateTimeOffset(DateTime.Now).ToUnixTimeSeconds())
             |> int)
        // secondary score is negative because secondary score is ordered as descending and less elapsed time is better
        { pairing with
            Player1 = fst pairing.Player1, { snd pairing.Player1 with Secondary = seconds }
            Player2 = fst pairing.Player2, { snd pairing.Player2 with Secondary = seconds } }
    | _ -> // pairing scores set to 0 ("unscored") -> reset Secondary as well
        { pairing with
            Player1 = fst pairing.Player1, { snd pairing.Player1 with Secondary = 0 }
            Player2 = fst pairing.Player2, { snd pairing.Player2 with Secondary = 0 } }

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
                      | "Enter", Some p -> dispatch (ConfirmScore(p |> secondary state.Round.Start))
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
            Html.span (fst pairing.Player1)
            Html.span [
                prop.children (
                    score
                        (snd pairing.Player1).Primary
                        [ prop.ref (fun node -> p1Ref.current <- node)
                          prop.onChange (fun num ->
                              dispatch (SetPlayer1Score { snd pairing.Player1 with Primary = num })) ]
                )
            ]
            Html.span (fst pairing.Player2)
            Html.span [
                prop.children (
                    score
                        (snd pairing.Player2).Primary
                        [ prop.onChange (fun num ->
                              dispatch (SetPlayer2Score { snd pairing.Player2 with Primary = num })) ]
                )
            ]
        ]
    ]

[<ReactComponent>]
let Round (state, dispatch) =
    React.contextProvider (
        context,
        (state, dispatch),
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
                                           |> List.map (fun p -> Pairing(p)))
                                    )
                                ]
                            ]
                        ]
                        Bulma.column [
                            column.isOneThird
                            prop.children [
                                Bulma.Divider.divider "Standings"
                                Components.Standings(
                                    rounds = [ state.Round ],
                                    total = state.StandingsAcc,
                                    aside =
                                        ("Time",
                                         (fun (player, _) ->
                                             let secondary =
                                                 match state.Round.Standings
                                                       |> List.tryFind (fun p -> (=) player (fst p))
                                                     with
                                                 | Some s -> (snd s).Secondary
                                                 | None -> 0

                                             Html.span (
                                                 System
                                                     .TimeSpan
                                                     .FromSeconds(secondary |> float |> Math.Abs)
                                                     .ToString("c", CultureInfo.InvariantCulture)
                                             )))
                                )
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
                              prop.onClick (fun _ -> dispatch (ConfirmScore(p |> secondary state.Round.Start)))
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
    )
