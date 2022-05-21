module Results.State

open Elmish
open Tournament.Round
open Tournament.Tournament
open Tournament.Player

type Status =
    | Ongoing
    | Finished
    | Confirmed

type ResultsModel =
    { Status: Status
      Rounds: Round list
      Bonus: Map<string, int>
      Standings: (string * int) list
      Form: (string * int) option }

type ResultsMsg =
    | Edit of (string * int) option
    | SetBonus of int
    | ConfirmBonus of (string * int)
    | Verify

let private getTotalScore (model) =
    model.Standings
    |> List.map (fun (p, s) -> (p, s + model.Bonus.[p]))
    |> List.sortBy (fun (_, score) -> -score)

let init (t: Tournament) =
    { Status = if t.Finished then Finished else Ongoing
      Rounds = t.Rounds
      Bonus = Map.ofList (t.Players |> List.map (fun p -> p.Name, 0))
      Standings = t.Standings()
      Form = None }

let nextEditable player list =
    match list
          |> List.findIndex (fun p -> ((=) (fst p) player))
        with
    | i when i <> (list.Length - 1) -> Some list.[i + 1]
    | _ -> None

let update msg model =
    match msg with
    | Edit p -> { model with Form = p }, Cmd.none
    | SetBonus num -> { model with Form = Some((fst model.Form.Value), num) }, Cmd.none
    | ConfirmBonus (p, s) ->
        { model with
            Form = nextEditable p model.Standings
            Bonus = model.Bonus |> Map.add p s },
        Cmd.none
    | Verify ->
        { model with
            Status = Confirmed
            Standings = getTotalScore model },
        Cmd.none
