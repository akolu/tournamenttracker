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
    |> List.filter (fun (name, _) -> List.exists (fun p -> p.Name = name) t.Players)
    |> List.map (fun res ->
        match List.tryFind (fun p -> p.Name = fst res) t.Players with
        | Some p -> (fst res, snd res + p.BonusScore)
        | None -> res)
    |> List.sortBy (fun (_, score) -> -score)

let init (t: Tournament) =
    { Rounds = t.Rounds
      Bonus = List.map (fun p -> p.Name, p.BonusScore) t.Players
      TotalScores = getTotalScore t
      Form = None }

let nextEditable player list =
    match list
          |> List.findIndex (fun p -> ((=) (fst p) (fst player)))
        with
    | i when i <> (list.Length - 1) -> Some list.[i + 1]
    | _ -> None

let update msg model =
    match msg with
    | Edit p -> { model with Form = p }, Cmd.none
    | SetBonus num -> { model with Form = Some((fst model.Form.Value), num) }, Cmd.none
    | ConfirmBonus p -> { model with Form = nextEditable p model.TotalScores }, Cmd.none
