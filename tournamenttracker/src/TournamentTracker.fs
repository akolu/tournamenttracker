module TournamentTracker

open Tournament.Tournament
open Tournament.PairingGenerator
open Tournament.Utils
open Fable.Core.JsInterop

// DTOs are anonymous records so that they get compiled into plain javascript objects instead of class instances
type PairingDTO =
    {| number: int
       player1: string
       player2: string
       player1Score: int
       player2Score: int |}

type RoundDTO =
    {| number: int
       pairings: PairingDTO []
       status: string |}

type TournamentDTO =
    {| players: string []
       rounds: RoundDTO [] |}

let private SerializePairing (p: Tournament.Pairing.Pairing) =
    {| number = p.Number
       player1 = p.Player1
       player2 = p.Player2
       player1Score = p.Player1Score
       player2Score = p.Player2Score |}

let private ParsePairing (p: obj) : Tournament.Pairing.Pairing =
    p :?> PairingDTO
    |> (fun p ->
        { Number = p.number
          Player1 = p.player1
          Player2 = p.player2
          Player1Score = p.player1Score
          Player2Score = p.player2Score })

let private ParseStatus s =
    match s with
    | "Pregame" -> Tournament.Round.Pregame
    | "Ongoing" -> Tournament.Round.Ongoing
    | "Finished" -> Tournament.Round.Finished
    | _ -> raise (System.Exception "invalid round status")

let private SerializeRound (r: Tournament.Round.Round) =
    {| number = r.Number
       pairings = List.toArray (List.map SerializePairing r.Pairings)
       status = r.Status.ToString() |}

let private ParseRound (r: obj) : Tournament.Round.Round =
    r :?> RoundDTO
    |> (fun r ->
        { Number = r.number
          Pairings = List.map ParsePairing (Array.toList r.pairings)
          Status = ParseStatus r.status })

let private SerializeTournament (t: Tournament) =
    {| players = List.toArray t.Players
       rounds = List.toArray (List.map SerializeRound t.Rounds) |}

let private ParseTournament (t: obj) =
    t :?> TournamentDTO
    |> (fun t ->
        { Players = Array.toList t.players
          Rounds = List.map ParseRound (Array.toList t.rounds) })

let private wrapSerialize (fn: Tournament -> Result<Tournament, string>) (tournament: obj) =
    tournament
    |> ParseTournament
    |> fn
    |> unwrap
    |> SerializeTournament

let createTournament rounds =
    createTournament rounds
    |> unwrap
    |> SerializeTournament

let addPlayers players tournament =
    wrapSerialize (addPlayers (players |> Array.toList)) tournament

let private parseAlg alg =
    match alg with
    | "Swiss"
    | "swiss" -> Swiss
    | "Shuffle"
    | "shuffle" -> Shuffle
    | _ -> raise (System.Exception(sprintf "Invalid pairing algorithm: %s" alg))

let pair alg tournament =
    wrapSerialize (pair (parseAlg alg)) tournament

let startRound tournament = wrapSerialize startRound tournament

let finishRound tournament = wrapSerialize finishRound tournament

let score number result tournament =
    wrapSerialize (score number result) tournament

let swap player1 player2 tournament =
    wrapSerialize (swap player1 player2) tournament

let standings tournament =
    tournament
    |> ParseTournament
    |> (fun t -> t.Standings)
    |> List.map (fun (name, score) -> {| player = name; score = score |})
    |> Array.ofList

let pairings tournament =
    tournament
    |> ParseTournament
    |> (fun t -> t.Pairings)
    |> List.map SerializePairing
    |> Array.ofList
