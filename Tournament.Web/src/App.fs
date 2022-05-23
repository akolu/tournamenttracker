module App

open Feliz
open Elmish
open Elmish.React
open Thoth.Json
open Browser.WebStorage

Fable.Core.JsInterop.importSideEffects "./App.scss"

type Model =
    { Tournament: Tournament.State.TournamentModel }

type Msg =
    | Save
    | TournamentMsg of Tournament.State.TournamentMsg

let state () =
    let model =
        match Decode.Auto.fromString<Tournament.State.TournamentModel> (localStorage.getItem "tournament") with
        | Ok t -> { Tournament = t }
        | Error _ ->
            localStorage.removeItem ("tournament")
            { Tournament = Tournament.State.init () }

    model, Cmd.none

let update msg model =
    match msg with
    | Save ->
        localStorage.setItem ("tournament", Encode.Auto.toString (0, model.Tournament))

        model, Cmd.none
    | TournamentMsg msg' ->
        let res, cmd = Tournament.State.update msg' model.Tournament

        { model with Tournament = res },
        Cmd.batch [
            Cmd.map TournamentMsg cmd
            Cmd.ofMsg Save
        ]

let view state dispatch =
    Html.div [
        Tournament.View.Tournament(state.Tournament, TournamentMsg >> dispatch)
    ]

Program.mkProgram state update view
|> Program.withReactSynchronous "elmish-app"
// |> Program.withConsoleTrace
|> Program.run
