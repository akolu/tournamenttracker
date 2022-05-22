module Results.View

open Feliz
open Feliz.Bulma
open Fable.FontAwesome
open State
open Components.Standings
open Browser.Types

Fable.Core.JsInterop.importSideEffects "./Styles.scss"

[<ReactComponent>]
let Results (state, dispatch) =

    let mutable refs =
        state.Standings
        |> List.map (fun (p, _) -> p, React.useRef<Browser.Types.Element> (null))
        |> dict

    React.useEffect (
        (fun _ ->
            match state.Editable with
            | Some player ->
                (unbox<HTMLInputElement> refs.[player].current)
                    .select ()
            | _ -> ()),
        [| box state.Editable |]
    )

    let renderBonus (player: string) =
        if state.Editable.IsSome then
            Html.input (
                [ prop.type' "number"
                  prop.onKeyDown (fun e ->
                      match e.key with
                      | "Enter" -> dispatch ConfirmBonus
                      | "Escape" -> dispatch CancelEditing
                      | _ -> ())
                  prop.onChange (fun (num: int) -> dispatch (SetBonus(player, num)))
                  prop.inputMode.numeric
                  prop.pattern (System.Text.RegularExpressions.Regex "[0-9]*")
                  prop.ref (fun node -> refs.[player].current <- node)
                  input.isSmall
                  prop.defaultValue state.Bonus.[player] ]
            )
        else
            Html.span (state.Bonus.[player])

    Html.div [
        prop.classes [
            "Results__root"
            if state.Editable.IsSome then
                "Results__root--editable"
        ]
        prop.children [
            Bulma.Divider.divider "Results"
            Components.Standings(
                rounds = state.Tournament.Rounds,
                total = state.Standings,
                onClick = (fun p -> dispatch (Edit p)),
                renderExtra = renderBonus
            )
            Html.span (
                if state.Editable.IsSome then
                    [ Bulma.button.button [
                          button.isSmall
                          button.isRounded
                          prop.hidden true
                          prop.onClick (fun _ -> dispatch ConfirmBonus)
                          prop.children [
                              Bulma.icon (Fa.i [ Fa.Solid.Check ] [])
                              Html.b "Confirm"
                          ]
                      ] ]
                else
                    [ Bulma.button.button [
                          button.isSmall
                          button.isRounded
                          prop.hidden true
                          prop.onClick (fun _ -> dispatch (Edit(fst state.Standings.Head)))
                          prop.children [
                              Bulma.icon (
                                  Html.i [
                                      prop.classes [
                                          "fa-regular"
                                          "fa-pen-to-square"
                                      ]
                                  ]
                              )
                              Html.b "Edit"
                          ]
                      ] ]
            )
        ]
    ]
