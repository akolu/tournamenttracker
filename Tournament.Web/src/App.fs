module App

open Feliz
open Elmish
open Elmish.React
open Components.TournamentContainer
open Context
open State

Fable.Core.JsInterop.importSideEffects "./App.scss"

let view (state: State) (dispatch: Action -> unit) =
    Html.div [
        React.contextProvider (tournamentContext, (state, dispatch), [ Components.TournamentContainer() ])
    ]

Program.mkProgram state update view
|> Program.withReactSynchronous "elmish-app"
// |> Program.withConsoleTrace
|> Program.run
