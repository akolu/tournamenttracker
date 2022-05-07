module Round.State

open Elmish
open Tournament.Pairing
open Tournament.Round

type RoundModel = { Round: Round; Form: Pairing option }

type RoundMsg =
    | Edit of Pairing option
    | SetPlayer1Score of int
    | SetPlayer2Score of int
    | ConfirmScore of Pairing
    | StartRound
    | FinishRound

let init (rnd: Round) = { Round = rnd; Form = None }, Cmd.none

let update msg model =
    match msg with
    | Edit p when model.Round.Status = Ongoing -> { model with Form = p }, Cmd.none
    | SetPlayer1Score e -> { model with Form = Some { model.Form.Value with Player1Score = e } }, Cmd.none
    | SetPlayer2Score e -> { model with Form = Some { model.Form.Value with Player2Score = e } }, Cmd.none
    | ConfirmScore _ -> { model with Form = None }, Cmd.none
    | _ -> model, Cmd.none
