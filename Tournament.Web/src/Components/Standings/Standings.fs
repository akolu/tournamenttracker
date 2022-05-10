module Components.Standings

open Feliz
open Feliz.Bulma
open Tournament.Round
open Tournament.Player

Fable.Core.JsInterop.importSideEffects "./Standings.scss"

[<ReactComponent>]
let Standings (rounds: Round list, total: (Player * int) list) =

    let getPlayerScore player (round: Round) =
        match round.Standings
              |> List.tryFind (fun p -> (=) player (fst p))
            with
        | Some s -> snd s
        | None -> 0

    Html.div [
        prop.className "Standings__root"
        prop.children (
            [ Html.div (
                  [ Html.span [
                        prop.children [
                            Html.aside [ Html.b "#" ]
                            Html.b "Player"
                        ]
                    ] ]
                  @ (rounds
                     |> List.map (fun r -> Html.b (sprintf "Round %d" r.Number)))
                    @ [ Html.span [
                            prop.children [
                                Html.aside [ Html.b "Extra" ]
                                Html.b "Total"
                            ]
                        ] ]
              ) ]
            @ (total
               |> List.mapi (fun i (player, score) ->
                   Html.div [
                       prop.children (
                           [ Html.span [
                                 prop.children [
                                     Html.aside [ Html.span (i + 1) ]
                                     Html.span player.Name
                                 ]
                             ] ]
                           @ (rounds
                              |> List.map (fun r -> Html.span (getPlayerScore player r)))
                             @ [ Html.span [
                                     prop.children [
                                         Html.aside [
                                             Html.input (
                                                 [ prop.type' "number"
                                                   //    prop.onKeyDown (fun e ->
                                                   //        match e.key, state.Form with
                                                   //        | "Enter", Some p -> dispatch (ConfirmScore p)
                                                   //        | "Escape", _ -> dispatch (Edit None)
                                                   //        | _ -> ())
                                                   prop.inputMode.numeric
                                                   prop.pattern (System.Text.RegularExpressions.Regex "[0-9]*")
                                                   input.isSmall
                                                   prop.defaultValue 0 ]
                                             )
                                         ]
                                         Html.span score
                                     ]
                                 ] ]
                       )
                   ]))
        )
    ]
