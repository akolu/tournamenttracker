module Components.Standings

open Feliz
open Tournament.Round

Fable.Core.JsInterop.importSideEffects "./Standings.scss"

type Components() =
    [<ReactComponent>]
    static member Standings(rounds, total, (?bonus: string -> ReactElement), (?onRowClick: string -> unit)) =

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
                      @ (rounds
                         |> List.map (fun r -> Html.b (sprintf "Round %d" r.Number)))
                        @ [ Html.span [
                                prop.children [
                                    match bonus with
                                    | Some _ -> Html.aside [ Html.b "Bonus" ]
                                    | None -> ()
                                    Html.b "Total"
                                ]
                            ] ]
                  ) ]
                @ (total
                   |> List.mapi (fun i (player, score: int) ->
                       Html.div [
                           prop.onClick (fun _ ->
                               match onRowClick with
                               | Some fn -> fn player
                               | None -> ())
                           prop.children (
                               [ Html.span [
                                     prop.custom ("data-ordinal", (sprintf "%d." (i + 1)))
                                     prop.text player
                                 ] ]
                               @ (rounds
                                  |> List.map (fun r -> Html.span (getPlayerScore player r)))
                                 @ [ Html.span [
                                         prop.children [
                                             match bonus with
                                             | Some fn -> Html.aside [ fn player ]
                                             | None -> ()
                                             Html.span score
                                         ]
                                     ] ]
                           )
                       ]))
            )
        ]
