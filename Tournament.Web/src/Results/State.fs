module Results.State

open Elmish
open Tournament.Round
open Tournament.Tournament
open Tournament.Player

type ResultsModel =
    { Rounds: Round list
      Extras: (Player * int) list
      TotalScores: (Player * int) list
      Form: (string * int) option }

type ResultsMsg =
    | Edit of (string * int) option
    | SetBonus of int
    | ConfirmBonus of (string * int)

let private getTotalScore (t: Tournament) =
    t.Standings()
    |> List.map (fun (p, s) -> (p, s + p.BonusScore))
    |> List.sortBy (fun (_, score) -> -score)

let init (t: Tournament) =
    { Rounds = t.Rounds
      Extras = List.map (fun p -> p, p.BonusScore) t.Players
      TotalScores = getTotalScore t
      Form = None },
    Cmd.none

let update msg model =
    match msg with
    | Edit p -> { model with Form = p }, Cmd.none
    | SetBonus num -> { model with Form = Some((fst model.Form.Value), num) }, Cmd.none
    | _ -> model, Cmd.none
