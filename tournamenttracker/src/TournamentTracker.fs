module TournamentTracker

open Fable.Core
open Tournament.Tournament
open Tournament.PairingGenerator
open Tournament.Utils

open Thoth.Json

let private serialize (tournament: Result<Tournament, string>) =
    Encode.Auto.toString (0, (tournament |> unwrap), CamelCase)

let private parse str =
    Decode.Auto.fromString<Tournament> (str, CamelCase)
    |> unwrap

let roundToJS (r: Tournament.Round.Round) =
    {| Number = r.Number
       Pairings = List.toArray (List.map JsInterop.toPlainJsObj r.Pairings)
       Status = r.Status.ToString() |}

let tournamentToJS (t: Tournament) =
    {| Players = List.toArray t.Players
       Rounds = List.toArray (List.map roundToJS t.Rounds) |}

let private wrapSerialize (fn: Tournament -> Result<Tournament, string>) tournament =
    parse tournament |> fn |> serialize

let createTournament rounds = createTournament rounds |> serialize

let createTournament2 rounds =
    Tournament.Tournament.createTournament rounds
    |> unwrap
    |> tournamentToJS

let addPlayers players tournament =
    wrapSerialize (addPlayers (players |> Array.toList)) tournament

let addPlayers2 players tournament =
    Tournament.Tournament.addPlayers (players |> Array.toList) tournament
    |> unwrap
    |> JsInterop.toPlainJsObj

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
    parse tournament
    |> (fun t -> t.Standings)
    |> (fun list -> Encode.Auto.toString (0, list, CamelCase))

let pairings tournament =
    parse tournament
    |> (fun t -> t.Pairings)
    |> (fun pairings -> Encode.Auto.toString (0, pairings, CamelCase))
