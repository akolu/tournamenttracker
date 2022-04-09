namespace Tournament

type Round =
    { Number: int
      Pairings: List<Pairing>
      Finished: bool }
    member this.Standings =
        this.Pairings
        |> List.map (fun p ->
            [ (p.Player1, p.Player1Score)
              (p.Player2, p.Player2Score) ])
        |> List.concat
        |> Map.ofList

module Round =

    let private replace fn item list =
        list
        |> List.map (fun i -> if fn i then item else i)

    let private modifyPairing (modified: Pairing) (rnd: Round) =
        { rnd with
            Pairings =
                (rnd.Pairings
                 |> replace (fun p -> ((=) p.Number modified.Number)) modified) }

    let addResults pairing round result =
        round
        |> modifyPairing
            { pairing with
                Player1Score = fst result
                Player2Score = snd result }
