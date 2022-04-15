namespace Tournament

open System

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
    type Pairings<'a> = List<Pair<'a * int>>

    let private getPairingMatrix (history: ('a * 'a) list) (players: ('a * int) list) =
        let playersHavePlayed (p1: 'a) (p2: 'a) =
            match history
                  |> List.tryFind (fun pair -> ((=) pair (p1, p2) || (=) pair (p2, p1)))
                with
            | Some _ -> true
            | None -> false

        let getPriority (p1: 'a * int) (p2: 'a * int) : int =
            let standings = List.sortBy (fun (name, score) -> -score, name) players

            [ p1; p2 ]
            |> List.map (fun player -> List.findIndex (fun p -> p = player) standings)
            |> List.sum

        players
        |> List.mapi (fun i p1 ->
            players
            |> List.skip i
            |> List.fold
                (fun acc p2 ->
                    match (playersHavePlayed (fst p1) (fst p2)) with
                    | true -> acc
                    | false ->
                        let sorted = List.sortBy (fun (n, s) -> -s, n) [ p1; p2 ]
                        // TODO: random player1 / player2 instead of having higher-ranking player always player1
                        acc
                        @ [ { P1 = sorted.[0]
                              P2 = sorted.[1]
                              Priority = (getPriority p1 p2) } ])
                []
            |> List.filter (fun pair -> pair.P1 <> pair.P2))
        |> List.fold (fun acc cur -> acc @ cur) []
        |> List.sortBy (fun pair -> pair.Priority)

    let private getNextAvailablePairing
        (paired: Pairings<'a>)
        (unpaired: Pairings<'a>)
        (blacklist: List<Pairings<'a>>)
        : option<Pair<'a * int>> =
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

    let swiss (history: ('a * 'a) list) (players: ('a * int) list) =
        let pairingMatrix = players |> getPairingMatrix history

        let allPlayersPaired (paired: List<Pair<'a * int>>) : bool =
            Seq.forall
                (fun player -> (List.exists (fun pair -> ((=) pair.P1 player) || ((=) pair.P2 player)) paired))
                players

        let rec getPairings
            (paired: Pairings<'a>)
            (blacklist: List<Pairings<'a>>)
            (unpaired: Pairings<'a>)
            : Pairings<'a> =
            // TODO: move blacklist check from getNextAvailablePairing to filterPossiblePairings
            match (getNextAvailablePairing paired unpaired blacklist) with
            | Some next ->
                getPairings (paired @ [ next ]) blacklist (filterPossiblePairings (paired @ [ next ]) unpaired)
            | None ->
                if (allPlayersPaired paired) = true then
                    paired
                else
                    getPairings
                        (paired |> List.except [ (List.last paired) ])
                        // TODO: find out better blacklist utilization (e.g. by blacklisting the whole pairing list instead of an individual pairing)
                        // this will likely not work in all circumstances if blacklisting a pairing earlier in the tree makes a previously blacklisted
                        // pairing "allowed" again
                        (blacklist @ [ paired ])
                        (filterPossiblePairings (paired |> List.except [ (List.last paired) ]) pairingMatrix)

        pairingMatrix
        |> (getPairings [] [])
        |> List.map (fun pair -> (pair.P1, pair.P2))
