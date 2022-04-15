namespace Tournament

type RoundStatus =
    | Pregame
    | Ongoing
    | Finished

type Round =
    { Number: int
      Pairings: List<Pairing>
      Status: RoundStatus }
    member this.Standings =
        this.Pairings
        |> List.map (fun p ->
            [ (p.Player1, p.Player1Score)
              (p.Player2, p.Player2Score) ])
        |> List.concat
        |> Map.ofList

module internal Round =

    let private replace fn item list =
        list
        |> List.map (fun i -> if fn i then item else i)

    let private modifyPairing (modified: Pairing) (rnd: Round) =
        { rnd with
            Pairings =
                (rnd.Pairings
                 |> replace (fun p -> ((=) p.Number modified.Number)) modified) }

    let addResults pairing result round =
        round
        |> modifyPairing
            { pairing with
                Player1Score = fst result
                Player2Score = snd result }

    let start (rnd: Round) =
        match rnd.Status with
        | Pregame when not rnd.Pairings.IsEmpty -> Ok { rnd with Status = Ongoing }
        | Pregame -> Error(sprintf "Unable to start round %i: no pairings" rnd.Number)
        | Ongoing -> Error(sprintf "Unable to start round %i: round already started" rnd.Number)
        | Finished -> Error(sprintf "Unable to start round %i: round already finished" rnd.Number)

// let finishRound (rnd: Round) =
