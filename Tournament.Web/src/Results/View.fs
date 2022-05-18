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

    let mutable p1Ref = React.useRef<Browser.Types.Element> (null)

    React.useEffect (
        (fun _ ->
            if (state.Form.IsSome) then
                (unbox<HTMLInputElement> p1Ref.current).select ()),
        [| box (fst (state.Form |> Option.defaultWith (fun () -> "", 0))) |]
    )

    let getBonus player =
        match List.tryFind (fun (p, _) -> p = player) state.Bonus with
        | Some (_, s) -> s
        | None -> 0

    let renderBonus player =
        match state.Form with
        | Some (p, s) when p = player ->
            Html.input (
                [ prop.type' "number"
                  prop.onKeyDown (fun e ->
                      match e.key with
                      | "Enter" -> dispatch (ConfirmBonus(p, s))
                      | "Escape" -> dispatch (Edit None)
                      | _ -> ())
                  prop.onChange (fun (num: int) -> dispatch (Edit(Some(p, num))))
                  prop.inputMode.numeric
                  prop.pattern (System.Text.RegularExpressions.Regex "[0-9]*")
                  prop.ref (fun node -> p1Ref.current <- node)
                  input.isSmall
                  prop.defaultValue s ]
            )
        | _ -> Html.span (getBonus player)

    Html.div [
        prop.classes [
            "Results__root"
            if state.Form.IsSome then
                "Results__root--editable"
        ]
        prop.children [
            Bulma.Divider.divider "Results"
            Components.Standings(
                rounds = state.Rounds,
                total = state.TotalScores,
                onClick = (fun p -> dispatch (Edit(Some(p, (getBonus p))))),
                renderExtra = renderBonus
            )
            Html.span (
                match state.Form with
                | (Some (p, s)) ->
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
                              Html.b "Confirm"
                          ]
                      ] ]
                | _ -> []
            )
        ]
    ]
