module Round.State

open Elmish
open Tournament.Pairing
open Tournament.Round
open Tournament.Tournament
open System

type RoundModel =
    { Round: Round
      Form: Pairing option
      StandingsAcc: (string * Score) list }

type RoundMsg =
    | Edit of Pairing option
    | SetPlayer1Score of Score
    | SetPlayer2Score of Score
    | SetSecondaryScore
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
    | SetPlayer1Score e ->
        (match model.Form with
         | Some f -> { model with Form = Some { f with Player1 = (fst f.Player1, e) } }
         | None -> model),
        Cmd.none
    | SetPlayer2Score e ->
        (match model.Form with
         | Some f -> { model with Form = Some { f with Player2 = (fst f.Player2, e) } }
         | None -> model),
        Cmd.none
    | ConfirmScore _ -> { model with Form = None }, Cmd.none
    | _ -> model, Cmd.none
