module Results.View

open Feliz
open Feliz.Bulma
open State
open Tournament.Player
open Components.Standings

Fable.Core.JsInterop.importSideEffects "./Styles.scss"

let Results (state: ResultsModel) (dispatch: ResultsMsg -> unit) =
    Html.div [
        prop.className "Results__root"
        prop.children [
            Bulma.Divider.divider "Results"
            Standings(
                {| rounds = state.Rounds
                   total = state.TotalScores
                   onRowClick = Some(fun (p, s) -> dispatch (Edit(Some(p.Name, s))))
                   extra =
                    Some (fun (player, _) ->
                        match state.Form with
                        | Some p when fst p = player.Name ->
                            Html.input (
                                [ prop.type' "number"
                                  prop.onKeyDown (fun e ->
                                      match e.key, state.Form with
                                      | "Enter", Some p -> dispatch (ConfirmBonus p)
                                      | "Escape", _ -> dispatch (Edit None)
                                      | _ -> ())
                                  prop.inputMode.numeric
                                  prop.pattern (System.Text.RegularExpressions.Regex "[0-9]*")
                                  input.isSmall
                                  prop.defaultValue player.BonusScore ]
                            )
                        | _ -> Html.span "-") |}
            )
        ]
    ]
