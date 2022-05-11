module Results.View

open Feliz
open Feliz.Bulma
open Tournament.Tournament
open Tournament.Player
open Components.Standings

Fable.Core.JsInterop.importSideEffects "./Styles.scss"

//     this.Standings()
//     |> List.map (fun (p, s) -> (p, s + p.BonusScore))
//     |> List.sortBy (fun (_, score) -> -score)

[<ReactComponent>]
let Results (tournament: Tournament) =
    Html.div [
        prop.className "Results__root"
        prop.children [
            Bulma.Divider.divider "Results"
            Standings(
                {| rounds = tournament.Rounds
                   total = tournament.Standings()
                   extra =
                    Some (fun (player, _) ->
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
                              prop.defaultValue player.BonusScore ]
                        )) |}
            )
        ]
    ]
