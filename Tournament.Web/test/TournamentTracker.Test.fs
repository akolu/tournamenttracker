module TournamentTracker.Test

open Fable.Jester
open TournamentTracker

let player (name: string) =
    {| name = name
       rating = 0
       bonusScore = 0 |}

Jest.describe (
    "TournamentTracker tests",
    fun () ->
        Jest.test (
            "can create tournament",
            (fun () ->
                let foo = createTournament 1

                Jest
                    .expect(foo)
                    .toEqual (
                        {| players = [||]
                           rounds =
                            [| {| number = 1
                                  pairings = [||]
                                  status = "Pregame" |} |] |}
                    ))
        )

        Jest.test (
            "can add players",
            (fun () ->
                let tournament =
                    createTournament 1
                    |> addPlayers [|
                        player "Ossi"
                        player "Aku"
                       |]

                Jest
                    .expect(tournament)
                    .toEqual (
                        {| players = [| player "Aku"; player "Ossi" |]
                           rounds =
                            [| {| number = 1
                                  pairings = [||]
                                  status = "Pregame" |} |] |}
                    )

                let withMorePlayers = tournament |> addPlayers [| player "Veikka" |]

                Jest
                    .expect(withMorePlayers)
                    .toEqual (
                        {| players =
                            [| player "Aku"
                               player "Ossi"
                               player "Veikka" |]
                           rounds =
                            [| {| number = 1
                                  pairings = [||]
                                  status = "Pregame" |} |] |}
                    ))
        )

        Jest.test (
            "can pair",
            (fun () ->
                let tournament =
                    createTournament 1
                    |> addPlayers [|
                        player "Ossi"
                        player "Aku"
                       |]
                    |> pair "Swiss"

                Jest
                    .expect(tournament)
                    .toEqual (
                        {| players = [| player "Aku"; player "Ossi" |]
                           rounds =
                            [| {| number = 1
                                  pairings =
                                   [| {| number = 1
                                         player1 = player "Aku"
                                         player2 = player "Ossi"
                                         player1Score = 0
                                         player2Score = 0 |} |]
                                  status = "Pregame" |} |] |}
                    ))
        )

        Jest.test (
            "can start round",
            (fun () ->
                let tournament =
                    createTournament 1
                    |> addPlayers [|
                        player "Ossi"
                        player "Aku"
                       |]
                    |> pair "Swiss"
                    |> startRound

                Jest
                    .expect(tournament)
                    .toEqual (
                        {| players = [| player "Aku"; player "Ossi" |]
                           rounds =
                            [| {| number = 1
                                  pairings =
                                   [| {| number = 1
                                         player1 = player "Aku"
                                         player2 = player "Ossi"
                                         player1Score = 0
                                         player2Score = 0 |} |]
                                  status = "Ongoing" |} |] |}
                    ))
        )

        Jest.test (
            "can score",
            (fun () ->
                let tournament =
                    createTournament 1
                    |> addPlayers [|
                        player "Ossi"
                        player "Aku"
                       |]
                    |> pair "Swiss"
                    |> startRound
                    |> score 1 (10, 10)

                Jest
                    .expect(tournament)
                    .toEqual (
                        {| players = [| player "Aku"; player "Ossi" |]
                           rounds =
                            [| {| number = 1
                                  pairings =
                                   [| {| number = 1
                                         player1 = player "Aku"
                                         player2 = player "Ossi"
                                         player1Score = 10
                                         player2Score = 10 |} |]
                                  status = "Ongoing" |} |] |}
                    ))
        )

        Jest.test (
            "can finish round",
            (fun () ->
                let tournament =
                    createTournament 1
                    |> addPlayers [|
                        player "Ossi"
                        player "Aku"
                       |]
                    |> pair "Swiss"
                    |> startRound
                    |> score 1 (20, 0)
                    |> finishRound


                Jest
                    .expect(tournament)
                    .toEqual (
                        {| players = [| player "Aku"; player "Ossi" |]
                           rounds =
                            [| {| number = 1
                                  pairings =
                                   [| {| number = 1
                                         player1 = player "Aku"
                                         player2 = player "Ossi"
                                         player1Score = 20
                                         player2Score = 0 |} |]
                                  status = "Finished" |} |] |}
                    ))
        )

        Jest.test (
            "can swap two players",
            (fun () ->
                let tournament =
                    createTournament 1
                    |> addPlayers [|
                        player "Aku"
                        player "Juha"
                        player "Ossi"
                        player "Veikka"
                       |]
                    |> pair "Swiss"
                    |> swap "Aku" "Ossi"

                Jest
                    .expect(tournament)
                    .toEqual (
                        {| players =
                            [| player "Aku"
                               player "Juha"
                               player "Ossi"
                               player "Veikka" |]
                           rounds =
                            [| {| number = 1
                                  pairings =
                                   [| {| number = 1
                                         player1 = player "Ossi"
                                         player2 = player "Juha"
                                         player1Score = 0
                                         player2Score = 0 |}
                                      {| number = 2
                                         player1 = player "Aku"
                                         player2 = player "Veikka"
                                         player1Score = 0
                                         player2Score = 0 |} |]
                                  status = "Pregame" |} |] |}
                    ))
        )

        Jest.test (
            "can display standings",
            (fun () ->
                let standings =
                    createTournament 1
                    |> addPlayers [|
                        player "Aku"
                        player "Juha"
                        player "Ossi"
                        player "Veikka"
                       |]
                    |> pair "Swiss"
                    |> startRound
                    |> score 1 (11, 9)
                    |> score 2 (5, 15)
                    |> finishRound
                    |> standings

                Jest
                    .expect(standings)
                    .toEqual (
                        [| {| player = player "Veikka"
                              score = 15 |}
                           {| player = player "Aku"; score = 11 |}
                           {| player = player "Juha"; score = 9 |}
                           {| player = player "Ossi"; score = 5 |} |]
                    ))
        )

        Jest.test (
            "can display pairings",
            (fun () ->
                let pairings =
                    createTournament 1
                    |> addPlayers [|
                        player "Aku"
                        player "Juha"
                        player "Ossi"
                        player "Veikka"
                       |]
                    |> pair "Swiss"
                    |> pairings

                Jest
                    .expect(pairings)
                    .toEqual (
                        [| {| number = 1
                              player1 = player "Aku"
                              player2 = player "Juha"
                              player1Score = 0
                              player2Score = 0 |}
                           {| number = 2
                              player1 = player "Ossi"
                              player2 = player "Veikka"
                              player1Score = 0
                              player2Score = 0 |} |]
                    ))
        )
)
