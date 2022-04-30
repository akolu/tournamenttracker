module App

open Feliz
open Elmish
open Elmish.React
open App.Components.TournamentContainer
open App.Context
open App.State

Fable.Core.JsInterop.importSideEffects "./App.scss"

let view (state: State) (dispatch: Msg -> unit) =
    Html.div [
        React.contextProvider (tournamentContext, (state, dispatch), [ TournamentContainer() ])
    ]

Program.mkProgram state update view
|> Program.withReactSynchronous "elmish-app"
|> Program.withConsoleTrace
|> Program.run
