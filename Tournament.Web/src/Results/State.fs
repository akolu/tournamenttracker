module Results.State

open Elmish
open Tournament.Round
open Tournament.Tournament
open Tournament.Player

type ResultsModel =
    { Rounds: Round list
      Bonus: (string * int) list
      TotalScores: (string * int) list
      Form: (string * int) option }

type ResultsMsg =
    | Edit of (string * int) option
    | SetBonus of int
    | ConfirmBonus of (string * int)

let private getTotalScore (t: Tournament) =
    t.Standings()
    |> List.map (fun res ->
        match List.tryFind (fun p -> p.Name = fst res) t.Players with
        | Some p -> (fst res, snd res + p.BonusScore)
        | None -> res)
    |> List.sortBy (fun (_, score) -> -score)


let init (t: Tournament) =
    { Rounds = t.Rounds
      Bonus = List.map (fun p -> p.Name, p.BonusScore) t.Players
      TotalScores = getTotalScore t
      Form = None },
    Cmd.none

let update msg model =
    match msg with
    | Edit p -> { model with Form = p }, Cmd.none
    | SetBonus num -> { model with Form = Some((fst model.Form.Value), num) }, Cmd.ofMsg (Edit None)
    | _ -> model, Cmd.none
