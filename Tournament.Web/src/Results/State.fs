module Results.State

open Elmish
open Tournament.Tournament
open Tournament.Player
open Tournament.Pairing

type ResultsModel =
    { Editable: string option
      Tournament: Tournament
      Bonus: Map<string, int>
      Standings: (string * Score) list }

type ResultsMsg =
    | Edit of string
    | CancelEditing
    | SetBonus of (string * int)
    | ConfirmBonus

let private getTotalScore (t: Tournament) =
    t.Standings()
    |> List.filter (fun (name, _) -> List.exists (fun p -> p.Name = name) t.Players)
    |> List.map (fun (player, score) ->
        match List.tryFind (fun p -> p.Name = player) t.Players with
        | Some p -> (player, { score with Primary = score.Primary + p.BonusScore })
        | None -> (player, score))
    |> List.sortBy (fun (p, score) -> -score.Primary, -score.Secondary, p)

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
