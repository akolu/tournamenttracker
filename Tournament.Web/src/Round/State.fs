module Round.State

open Elmish
open Tournament.Pairing
open Tournament.Round
open Tournament.Tournament

type RoundModel =
    { Round: Round
      Form: Pairing option
      StandingsAcc: (string * int) list }

type RoundMsg =
    | Edit of Pairing option
    | SetPlayer1Score of int
    | SetPlayer2Score of int
    | ConfirmScore of Pairing
    | StartRound
    | FinishRound

let init num t =
    { Round = List.find (fun r -> r.Number = num) t.Rounds
      Form = None
      StandingsAcc = t.Standings num }

let update msg model =
    match msg with
    | Edit p when model.Round.Status = Ongoing ->
        match p, model.Form with
        | Some a, Some b when ((=) a.Number b.Number) -> model, Cmd.none // pairing already in edit mode
        | _, Some b ->
            { model with Form = None },
            Cmd.batch [
                Cmd.ofMsg (ConfirmScore b)
                Cmd.ofMsg (Edit p)
            ]
        | _ -> { model with Form = p }, Cmd.none
    // TODO: refactor direct option value access -> change to pattern matching
    | SetPlayer1Score e -> { model with Form = Some { model.Form.Value with Player1Score = e } }, Cmd.none
    | SetPlayer2Score e -> { model with Form = Some { model.Form.Value with Player2Score = e } }, Cmd.none
    | ConfirmScore _ -> { model with Form = None }, Cmd.none
    | _ -> model, Cmd.none
