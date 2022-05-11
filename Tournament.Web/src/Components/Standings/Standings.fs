module Components.Standings

open Feliz
open Tournament.Round
open Tournament.Player

Fable.Core.JsInterop.importSideEffects "./Standings.scss"

[<ReactComponent>]
let Standings
    (props: {| rounds: Round list
               total: (Player * int) list
               extra: option<Player * int -> ReactElement>
               onRowClick: option<(Player * int) -> unit> |})
    =

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
                        prop.children [ Html.b "Player" ]
                    ] ]
                  @ (props.rounds
                     |> List.map (fun r -> Html.b (sprintf "Round %d" r.Number)))
                    @ [ Html.span [
                            prop.children [
                                match props.extra with
                                | Some _ -> Html.aside [ Html.b "Extra" ]
                                | None -> ()
                                Html.b "Total"
                            ]
                        ] ]
              ) ]
            @ (props.total
               |> List.mapi (fun i (player, score) ->
                   Html.div [
                       prop.onClick (fun _ ->
                           match props.onRowClick with
                           | Some fn -> fn (player, score)
                           | None -> ())
                       prop.children (
                           [ Html.span [
                                 prop.custom ("data-ordinal", (sprintf "%d." (i + 1)))
                                 prop.text player.Name
                             ] ]
                           @ (props.rounds
                              |> List.map (fun r -> Html.span (getPlayerScore player r)))
                             @ [ Html.span [
                                     prop.children [
                                         match props.extra with
                                         | Some fn -> Html.aside [ fn (player, score) ]
                                         | None -> ()
                                         Html.span score
                                     ]
                                 ] ]
                       )
                   ]))
        )
    ]
