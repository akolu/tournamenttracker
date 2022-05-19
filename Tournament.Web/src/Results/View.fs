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

    let mutable ref = React.useRef<Browser.Types.Element> (null)

    React.useEffect (
        (fun _ ->
            if (state.Form.IsSome) then
                (unbox<HTMLInputElement> ref.current).select ()),
        [| box (fst (state.Form |> Option.defaultWith (fun () -> "", 0))) |]
    )

    let renderBonus player =
        match state.Form with
        | Some (p, s) when p = player ->
            Html.input (
                [ prop.type' "number"
                  prop.onKeyDown (fun e ->
                      match e.key with
                      | "Enter" -> dispatch (ConfirmBonus(p, (unbox<HTMLInputElement> ref.current).value |> int))
                      | "Escape" -> dispatch (Edit None)
                      | _ -> ())
                  prop.onChange (fun (num: int) -> dispatch (Edit(Some(p, num))))
                  prop.inputMode.numeric
                  prop.pattern (System.Text.RegularExpressions.Regex "[0-9]*")
                  prop.ref (fun node -> ref.current <- node)
                  input.isSmall
                  prop.defaultValue state.Bonus.[p] ]
            )
        | _ -> Html.span (state.Bonus.[player])

    Html.div [
        prop.classes [
            "Results__root"
            match state.Status with
            | Confirmed -> "Results__root--confirmed"
            | _ -> "Results__root--unconfirmed"
        ]
        prop.children [
            Bulma.Divider.divider "Results"
            Components.Standings(
                rounds = state.Rounds,
                total = state.Standings,
                onClick =
                    (fun p ->
                        if state.Status <> Confirmed then
                            dispatch (Edit(Some(p, (state.Bonus.[p]))))),
                renderExtra = renderBonus
            )
            Html.span (
                match (state.Status, state.Form) with
                | (_, Some (p, s)) ->
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
                          prop.onClick (fun _ -> dispatch (ConfirmBonus(p, s)))
                          prop.children [
                              Bulma.icon (Fa.i [ Fa.Solid.Check ] [])
                              Html.b "Ok"
                          ]
                      ] ]
                | Finished, None ->
                    [ Bulma.button.button [
                          button.isSmall
                          button.isRounded
                          prop.onClick (fun _ -> dispatch Verify)
                          prop.children [
                              Bulma.icon (Fa.i [ Fa.Solid.FlagCheckered ] [])
                              Html.b "Tournament finished"
                          ]
                      ] ]
                | _ -> []
            )
        ]
    ]
