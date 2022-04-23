module App

(**
 The famous Increment/Decrement ported from Elm.
 You can find more info about Elmish architecture and samples at https://elmish.github.io/
*)

open Feliz
open Elmish
open Elmish.React
open App.Components.TournamentContainer
open App.Context
open App.State

let view (state: State) (dispatch: Msg -> unit) =
    Html.div [ React.contextProvider (counterContext, (state, dispatch), [ TournamentContainer() ]) ]

// App
Program.mkProgram state update view
|> Program.withReactSynchronous "elmish-app"
|> Program.withConsoleTrace
|> Program.run
