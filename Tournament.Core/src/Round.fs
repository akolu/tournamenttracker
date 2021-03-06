module Tournament.Round

open Utils
open Pairing
open System

type RoundStatus =
    | Pregame
    | Ongoing
    | Finished

type Round =
    { Number: int
      Pairings: List<Pairing>
      Start: DateTime option
      Finish: DateTime option }
    member this.Standings =
        this.Pairings
        |> List.map (fun p -> [ p.Player1; p.Player2 ])
        |> List.concat
        |> List.sortBy (fun (p, score) -> -score.Primary, -score.Secondary, p)

    member this.Status =
        match this.Start, this.Finish with
        | Some _, Some _ -> Finished
        | Some _, _ -> Ongoing
        | _ -> Pregame

let private pairsToPairings pairs =
    pairs
    |> List.mapi (fun i ((p1Name, _), (p2Name, _)) ->
        { Number = (+) i 1
          Player1 = p1Name, Score.Empty
          Player2 = p2Name, Score.Empty })

// TODO: change fn to return Result
let internal createPairings fn (standings: List<string * Score>) (round: Round) =
    let playerList =
        match standings with
        | list when (<>) ((%) list.Length 2) 0 -> standings @ [ ("BYE", Score.Empty) ]
        | _ -> standings

    if round.Status = Pregame then
        Ok { round with Pairings = (fn playerList) |> pairsToPairings }
    else
        Error(sprintf "Unable to pair: round %i already started" round.Number)

let private validatePlayerSwap player round =
    let exists player pairing =
        ((=) player (fst pairing.Player1))
        || ((=) player (fst pairing.Player2))

    match List.tryFind (exists player) round.Pairings with
    | Some p when fst p.Player1 = player -> Ok p.Player1
    | Some p when fst p.Player2 = player -> Ok p.Player2
    | _ -> Error(sprintf "Player %s not found" player)

let internal swapPlayers player1 player2 round =
    let trySwap p1 p2 pairing =
        let tryReplace =
            function
            | p when ((=) p1 p) -> p2
            | p when ((=) p2 p) -> p1
            | p -> p

        { pairing with
            Player1 = (tryReplace pairing.Player1)
            Player2 = (tryReplace pairing.Player2) }

    match (validatePlayerSwap player1 round, validatePlayerSwap player2 round) with
    | _ when round.Status <> Pregame -> Error "Can't swap players: round already started!"
    | (Ok p1, Ok p2) -> Ok { round with Pairings = List.map (trySwap p1 p2) round.Pairings }
    | (Error err, _) -> Error err
    | (_, Error err) -> Error err

let internal start (round: Round) =
    match round.Status with
    | Pregame when not round.Pairings.IsEmpty -> Ok { round with Start = Some DateTime.Now }
    | Pregame -> Error(sprintf "Unable to start round %i: no pairings" round.Number)
    | Ongoing -> Error(sprintf "Unable to start round %i: round already started" round.Number)
    | Finished -> Error(sprintf "Unable to start round %i: round already finished" round.Number)

let internal finish (round: Round) =
    match round.Status with
    | Pregame -> Error(sprintf "Unable to finish round %i: round not started" round.Number)
    | Ongoing when Seq.forall (fun (p: Pairing) -> p.IsScored) round.Pairings ->
        Ok { round with Finish = Some DateTime.Now }
    | Ongoing -> Error(sprintf "Unable to finish round %i: unscored pairings exist" round.Number)
    | Finished -> Error(sprintf "Unable to finish round %i: round already finished" round.Number)

let internal score number result round =
    match (List.tryFind (fun (p: Pairing) -> number = p.Number) round.Pairings) with
    | _ when round.Status <> Ongoing -> Error(sprintf "Unable to score: round %i not started" round.Number)
    | Some pairing -> Ok { round with Pairings = (replace ((=) pairing) (pairing.Score result)) round.Pairings }
    | None -> Error(sprintf "Match %i not found!" number)
