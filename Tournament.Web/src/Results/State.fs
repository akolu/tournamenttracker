module Results.State

open Elmish
open Tournament.Round
open Tournament.Tournament
open Tournament.Player

type ResultsModel =
    { Editable: string option
      Tournament: Tournament
      Bonus: Map<string, int>
      Standings: (string * int) list }

type ResultsMsg =
    | Edit of string
    | CancelEditing
    | SetBonus of (string * int)
    | ConfirmBonus

let private getTotalScore (t: Tournament) =
    t.Standings()
    |> List.filter (fun (name, _) -> List.exists (fun p -> p.Name = name) t.Players)
    |> List.map (fun res ->
        match List.tryFind (fun p -> p.Name = fst res) t.Players with
        | Some p -> (fst res, snd res + p.BonusScore)
        | None -> res)
    |> List.sortBy (fun (_, score) -> -score)

let init (t: Tournament) =
    { Editable = None
      Tournament = t
      Bonus =
        t.Players
        |> List.map (fun p -> p.Name, p.BonusScore)
        |> Map.ofList
      Standings = getTotalScore t }

let update msg model =
    match msg with
    | Edit player -> { model with Editable = Some player }, Cmd.none
    | CancelEditing ->
        { model with
            Editable = None
            Bonus =
                model.Tournament.Players
                |> List.map (fun p -> p.Name, p.BonusScore)
                |> Map.ofList },
        Cmd.none
    | SetBonus (p, s) -> { model with Bonus = model.Bonus |> Map.add p s }, Cmd.none
    | ConfirmBonus -> model, Cmd.none
