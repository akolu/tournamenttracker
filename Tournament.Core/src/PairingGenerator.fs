namespace Tournament

open System
open Pairing

module PairingGenerator =
    type PairingAlgorithm =
        | Shuffle
        | Swiss

    let shuffle (items: 'a list) : ('a * 'a) list =
        items
        |> List.sortBy (fun _ -> Guid.NewGuid())
        |> List.chunkBySize 2
        |> List.fold (fun acc chunk -> acc @ [ (chunk.[0], chunk.[1]) ]) []

    type Pair<'a> = { P1: 'a; P2: 'a; Priority: int }
    type Pairings<'a> = List<Pair<'a * Score>>

    let private getPairingMatrix (history: ('a * 'a) list) (players: ('a * Score) list) =
        let playersHavePlayed (p1: 'a) (p2: 'a) =
            match history
                  |> List.tryFind (fun pair -> ((=) pair (p1, p2) || (=) pair (p2, p1)))
                with
            | Some _ -> true
            | None -> false

        let getPriority (p1: 'a * Score) (p2: 'a * Score) : int =
            let standings =
                List.sortBy (fun (name, score) -> -score.Primary, -score.Secondary, name) players

            [ p1; p2 ]
            |> List.map (fun player -> List.findIndex (fun p -> p = player) standings)
            |> List.sum

        players
        |> List.allPairs players
        |> List.filter (fun (a, b) -> a <> b && not (playersHavePlayed (fst a) (fst b)))
        |> List.map (fun (a, b) ->
            let sorted = List.sortBy (fun (n, s) -> -s.Primary, -s.Secondary, n) [ a; b ]
            // TODO: random player1 / player2 instead of having higher-ranking player always player1
            { P1 = sorted.[0]
              P2 = sorted.[1]
              Priority = (getPriority a b) })
        |> List.sortBy (fun pair -> pair.Priority)

    let private getNextAvailablePairing
        (paired: Pairings<'a>)
        (unpaired: Pairings<'a>)
        (blacklist: List<Pairings<'a>>)
        : option<Pair<'a * Score>> =
        List.tryFind (fun pair -> not (List.exists (fun list -> (list = (paired @ [ pair ]))) blacklist)) unpaired

    let private filterPossiblePairings (paired: Pairings<'a>) (unpaired: Pairings<'a>) : Pairings<'a> =
        unpaired
        |> List.filter (fun pair1 ->
            not (
                paired
                |> List.exists (fun pair2 ->
                    (pair1.P1 = pair2.P1)
                    || (pair1.P2 = pair2.P2)
                    || pair1.P1 = pair2.P2
                    || pair1.P2 = pair2.P1)
            ))

    let swiss (history: ('a * 'a) list) (players: ('a * Score) list) =
        let pairingMatrix = players |> getPairingMatrix history

        let allPlayersPaired (paired: List<Pair<'a * Score>>) : bool =
            Seq.forall
                (fun player -> (List.exists (fun pair -> ((=) pair.P1 player) || ((=) pair.P2 player)) paired))
                players

        let rec getPairings
            (paired: Pairings<'a>)
            (blacklist: List<Pairings<'a>>)
            (unpaired: Pairings<'a>)
            : Pairings<'a> =
            match (getNextAvailablePairing paired unpaired blacklist) with
            | Some next ->
                getPairings (paired @ [ next ]) blacklist (filterPossiblePairings (paired @ [ next ]) unpaired)
            | None ->
                if (allPlayersPaired paired) = true then
                    paired
                else
                    getPairings
                        (paired |> List.except [ (List.last paired) ])
                        (blacklist @ [ paired ])
                        (filterPossiblePairings (paired |> List.except [ (List.last paired) ]) pairingMatrix)

        pairingMatrix
        |> (getPairings [] [])
        |> List.map (fun pair -> (pair.P1, pair.P2))
