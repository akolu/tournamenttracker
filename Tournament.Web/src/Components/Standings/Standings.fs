module Components.Standings

open Feliz
open Tournament.Round

Fable.Core.JsInterop.importSideEffects "./Standings.scss"

[<ReactComponent>]
let Standings (rounds: Round list, total: (string * int) list) =

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
                  [ Html.b [
                        prop.children [
                            Html.aside [ Html.b "#" ]
                            Html.b "Player"
                        ]
                    ] ]
                  @ (rounds
                     |> List.map (fun r -> Html.b (sprintf "Round %d" r.Number)))
                    @ [ Html.b "Total" ]
              ) ]
            @ (total
               |> List.mapi (fun i (player, total) ->
                   Html.div [
                       prop.children (
                           [ Html.span [
                                 prop.children [
                                     Html.aside [ Html.span (i + 1) ]
                                     Html.span player
                                 ]
                             ] ]
                           @ (rounds
                              |> List.map (fun r -> Html.span (getPlayerScore player r)))
                             @ [ Html.span total ]
                       )
                   ]))
        )
    ]
