module Components.Standings

open Feliz
open Tournament.Round

Fable.Core.JsInterop.importSideEffects "./Standings.scss"

type AsideRenderFn = (string * Tournament.Pairing.Score) -> ReactElement

type Components() =
    [<ReactComponent>]
    static member Standings(rounds, total, (?aside: string * AsideRenderFn), (?onClick: string -> unit)) =

        let getPlayerScore player (round: Round) =
            match round.Standings
                  |> List.tryFind (fun p -> (=) player (fst p))
                with
            | Some s -> (snd s).Primary
            | None -> 0

        Html.div [
            prop.className "Standings__root"
            prop.children (
                [ Html.div (
                      [ Html.span [
                            prop.children [ Html.b "Player" ]
                        ] ]
                      @ (rounds |> List.map (fun r -> Html.b "Score"))
                        @ [ Html.span [
                                prop.children [
                                    match aside with
                                    | Some (t, _) -> Html.aside [ Html.b t ]
                                    | None -> ()
                                    Html.b "Total"
                                ]
                            ] ]
                  ) ]
                @ (total
                   |> List.mapi (fun i (player, score: Tournament.Pairing.Score) ->
                       Html.div [
                           prop.onClick (fun _ ->
                               match onClick with
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
                                             match aside with
                                             | Some (_, fn) -> Html.aside [ fn (player, score) ]
                                             | None -> ()
                                             Html.span score.Primary
                                         ]
                                     ] ]
                           )
                       ]))
            )
        ]
