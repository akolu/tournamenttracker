module Settings.State

open Elmish
open Tournament.Tournament

type ValidationFunction =
    | Players
    | Rounds

type SettingsModel =
    { Editable: bool
      Rounds: int
      Players: (string * int) list
      ValidationErrors: Map<ValidationFunction, string> }

type SettingsMsg =
    | AddRounds
    | RemoveRounds
    | AddPlayers
    | RemovePlayers
    | EditPlayerName of (int * string)
    | Confirm
    | Validate of ValidationFunction
    | Reset

let private getPlayers (t: Tournament) =
    if List.isEmpty t.Players then
        [ ("", 0); ("", 0) ]
    else
        t.Players |> List.map (fun p -> p.Name, p.Rating)

let init (t: Tournament) =
    { Editable = t = Tournament.Empty
      Rounds = max 1 t.Rounds.Length
      Players = getPlayers t
      ValidationErrors = Map.empty },
    Cmd.batch [
        Cmd.ofMsg (Validate Players)
        Cmd.ofMsg (Validate Rounds)
    ]

let private uniqueSwissPairingsPossible model =
    if (model.Rounds
        >= (((model.Players.Length |> float) / 2.0)
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

let private validate fn model =
    match fn with
    | Rounds -> uniqueSwissPairingsPossible model
    | Players -> allPlayersNamedWithUniqueNames model

let private getValidationErrors fn model =
    match validate fn model with
    | Ok model -> { model with ValidationErrors = model.ValidationErrors |> Map.remove fn }
    | Error err -> { model with ValidationErrors = model.ValidationErrors |> Map.add fn err }

let update msg model =
    match msg with
    | AddRounds when model.Rounds < 9 -> { model with Rounds = model.Rounds + 1 }, Cmd.ofMsg (Validate Rounds)
    | AddRounds -> model, Cmd.none
    | RemoveRounds when model.Rounds > 1 -> { model with Rounds = model.Rounds - 1 }, Cmd.ofMsg (Validate Rounds)
    | RemoveRounds -> model, Cmd.none
    | AddPlayers ->
        { model with Players = model.Players @ [ ("", 0) ] },
        Cmd.batch [
            Cmd.ofMsg (Validate Players)
            Cmd.ofMsg (Validate Rounds)
        ]
    | RemovePlayers when model.Players.Length > 2 ->
        { model with Players = model.Players.[.. model.Players.Length - 2] },
        Cmd.batch [
            Cmd.ofMsg (Validate Players)
            Cmd.ofMsg (Validate Rounds)
        ]
    | RemovePlayers ->
        model,
        Cmd.batch [
            Cmd.ofMsg (Validate Players)
            Cmd.ofMsg (Validate Rounds)
        ]
    | EditPlayerName (index, name) ->
        { model with
            Players =
                model.Players
                |> List.mapi (fun i p -> if i = index then (name, snd p) else p) },
        Cmd.ofMsg (Validate Players)
    | Confirm -> model, Cmd.none
    | Validate fn -> model |> getValidationErrors fn, Cmd.none
    | _ -> model, Cmd.none // reset handled by parent update fn
