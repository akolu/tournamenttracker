module Settings.State

open Elmish
open Tournament.Utils
open Tournament.Tournament

type SettingsModel =
    { Rounds: int
      Players: (string * int) list
      ValidationErrors: string list }

type SettingsMsg =
    | AddRounds
    | RemoveRounds
    | AddPlayers
    | RemovePlayers
    | EditPlayerName of (int * string)
    | Confirm
    | ValidateSettings

let init rounds players =
    { Rounds = rounds
      Players = players |> List.map (fun p -> (p, 0))
      ValidationErrors = [] },
    Cmd.none

let private uniqueSwissPairingsPossible model =
    if (model.Rounds < (((model.Players.Length |> float) / 2.0)
                        |> System.Math.Ceiling
                        |> int
                        |> (*) 2)) then
        Error "Unique Swiss pairings not possible with these"
    else
        Ok model

let private allPlayersNamedWithUniqueNames model =
    if ((model.Players
         |> List.map (fun p -> fst p)
         |> List.filter (fun p -> p.Trim() <> "")
         |> Set.ofList)
            .Count
        <> model.Players.Length) then
        Error "All players must be named, duplicate names are not allowed"
    else
        Ok model

let private validate model =
    model
    |> uniqueSwissPairingsPossible
    >>= allPlayersNamedWithUniqueNames

let private getValidationErrors model =
    match validate model with
    | Ok model -> model
    | Error err -> { model with ValidationErrors = [ err ] }

let update msg model =
    match msg with
    | AddRounds when model.Rounds < 9 -> { model with Rounds = model.Rounds + 1 }, Cmd.none
    | AddRounds -> model, Cmd.none
    | RemoveRounds when model.Rounds > 1 -> { model with Rounds = model.Rounds - 1 }, Cmd.none
    | RemoveRounds -> model, Cmd.none
    | AddPlayers -> { model with Players = model.Players @ [ ("", 0) ] }, Cmd.none
    | RemovePlayers when model.Players.Length > 2 ->
        { model with Players = model.Players.[.. model.Players.Length - 2] }, Cmd.none
    | RemovePlayers -> model, Cmd.none
    | EditPlayerName (index, name) ->
        { model with
            Players =
                model.Players
                |> List.mapi (fun i p -> if i = index then (name, snd p) else p) },
        Cmd.none
    | Confirm -> model, Cmd.none
    | ValidateSettings -> model, Cmd.none
