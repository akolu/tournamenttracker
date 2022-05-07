module Settings.State

open Elmish
open Tournament.Tournament

type SettingsModel =
    { Rounds: int
      Players: (string * int) list }

type SettingsMsg =
    | AddRounds
    | RemoveRounds
    | AddPlayers
    | RemovePlayers
    | EditPlayerName of (int * string)
    | Confirm

let init rounds players =
    { Rounds = rounds
      Players = players |> List.map (fun p -> (p, 0)) },
    Cmd.none

let update msg model =
    match msg with
    | AddRounds when model.Rounds < 9 -> { model with Rounds = model.Rounds + 1 }, Cmd.none
    | AddRounds -> model, Cmd.none
    | RemoveRounds when model.Rounds > 1 -> { model with Rounds = model.Rounds - 1 }, Cmd.none
    | RemoveRounds -> model, Cmd.none
    | AddPlayers -> { model with Players = model.Players @ [ ("", 0) ] }, Cmd.none
    | RemovePlayers when model.Players.Length > 1 ->
        { model with Players = model.Players.[.. model.Players.Length - 2] }, Cmd.none
    | RemovePlayers -> model, Cmd.none
    | EditPlayerName (index, name) ->
        { model with
            Players =
                model.Players
                |> List.mapi (fun i p -> if i = index then (name, snd p) else p) },
        Cmd.none
    | Confirm -> model, Cmd.none
