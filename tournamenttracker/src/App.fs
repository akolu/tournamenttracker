module App

(**
 The famous Increment/Decrement ported from Elm.
 You can find more info about Elmish architecture and samples at https://elmish.github.io/
*)

open Feliz
open Elmish
open Elmish.React
open Components.HelloWorld

// MODEL

type State = { Count: int }

type Msg =
    | Increment
    | Decrement

let init () = { Count = 0 }, Cmd.none

// UPDATE

let update (msg: Msg) (state: State) =
    match msg with
    | Increment -> { state with Count = state.Count + 1 }, Cmd.none
    | Decrement -> { state with Count = state.Count - 1 }, Cmd.none

// VIEW (rendered with React)

let view (state: State) (dispatch: Msg -> unit) =
    Html.div [ HelloWorld()

               Html.button [ prop.onClick (fun _ -> dispatch Increment)
                             prop.text "Increment" ]

               Html.button [ prop.onClick (fun _ -> dispatch Decrement)
                             prop.text "Decrement" ]

               Html.h1 state.Count ]

// App
Program.mkProgram init update view
|> Program.withReactSynchronous "elmish-app"
|> Program.withConsoleTrace
|> Program.run
