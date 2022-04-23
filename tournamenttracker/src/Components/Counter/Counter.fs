module App.Components.Counter

open Feliz
open App.State
open App.Context

[<ReactComponent>]
let Counter () =
    let (state, dispatch) = React.useContext (counterContext)

    Html.div [
        Html.button [
            prop.onClick (fun _ -> dispatch Increment)
            prop.text "Increment"
        ]

        Html.button [
            prop.onClick (fun _ -> dispatch Decrement)
            prop.text "Decrement"
        ]

        Html.h1 state.Count
    ]
