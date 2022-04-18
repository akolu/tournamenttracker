module TournamentTracker.Test

open Fable.Jester
open Tournament
open Tournament.Tournament

Jest.describe (
    "TournamentTracker tests",
    fun () ->
        Jest.test (
            "can create tournament",
            (fun () ->
                let tournament = createTournamentJson 1

                Jest
                    .expect(tournament)
                    .toEqual ("{\"rounds\":[{\"number\":1,\"pairings\":[],\"status\":\"Pregame\"}],\"players\":[]}"))
        )

        Jest.test (
            "can add players",
            (fun () ->
                let tournament =
                    createTournamentJson 1
                    |> addPlayersJson [| "Ossi"; "Aku" |]

                Jest
                    .expect(tournament)
                    .toEqual (
                        "{\"rounds\":[{\"number\":1,\"pairings\":[],\"status\":\"Pregame\"}],\"players\":[\"Aku\",\"Ossi\"]}"
                    )

                let withMorePlayers = tournament |> addPlayersJson [| "Veikka" |]

                Jest
                    .expect(withMorePlayers)
                    .toEqual (
                        "{\"rounds\":[{\"number\":1,\"pairings\":[],\"status\":\"Pregame\"}],\"players\":[\"Aku\",\"Ossi\",\"Veikka\"]}"
                    ))
        )

        Jest.test (
            "can pair",
            (fun () ->
                let tournament =
                    createTournamentJson 1
                    |> addPlayersJson [| "Ossi"; "Aku" |]
                    |> pairJson "Swiss"

                Jest
                    .expect(tournament)
                    .toEqual (
                        "{\"rounds\":[{\"number\":1,\"pairings\":[{\"number\":0,\"player1\":\"Aku\",\"player2\":\"Ossi\",\"player1Score\":0,\"player2Score\":0}],\"status\":\"Pregame\"}],\"players\":[\"Aku\",\"Ossi\"]}"
                    ))
        )

        Jest.test (
            "can start round",
            (fun () ->
                let tournament =
                    createTournamentJson 1
                    |> addPlayersJson [| "Ossi"; "Aku" |]
                    |> pairJson "Swiss"
                    |> startRoundJson

                Jest
                    .expect(tournament)
                    .toEqual (
                        "{\"rounds\":[{\"number\":1,\"pairings\":[{\"number\":0,\"player1\":\"Aku\",\"player2\":\"Ossi\",\"player1Score\":0,\"player2Score\":0}],\"status\":\"Ongoing\"}],\"players\":[\"Aku\",\"Ossi\"]}"
                    ))
        )

        Jest.test (
            "can score",
            (fun () ->
                let tournament =
                    createTournamentJson 1
                    |> addPlayersJson [| "Ossi"; "Aku" |]
                    |> pairJson "Swiss"
                    |> startRoundJson
                    |> scoreJson 0 (10, 10)

                Jest
                    .expect(tournament)
                    .toEqual (
                        "{\"rounds\":[{\"number\":1,\"pairings\":[{\"number\":0,\"player1\":\"Aku\",\"player2\":\"Ossi\",\"player1Score\":10,\"player2Score\":10}],\"status\":\"Ongoing\"}],\"players\":[\"Aku\",\"Ossi\"]}"
                    ))
        )

        Jest.test (
            "can finish round",
            (fun () ->
                let tournament =
                    createTournamentJson 1
                    |> addPlayersJson [| "Ossi"; "Aku" |]
                    |> pairJson "Swiss"
                    |> startRoundJson
                    |> scoreJson 0 (20, 0)
                    |> finishRoundJson


                Jest
                    .expect(tournament)
                    .toEqual (
                        "{\"rounds\":[{\"number\":1,\"pairings\":[{\"number\":0,\"player1\":\"Aku\",\"player2\":\"Ossi\",\"player1Score\":20,\"player2Score\":0}],\"status\":\"Finished\"}],\"players\":[\"Aku\",\"Ossi\"]}"
                    ))
        )

        Jest.test (
            "can swap two players",
            (fun () ->
                let tournament =
                    createTournamentJson 1
                    |> addPlayersJson [| "Aku"
                                         "Juha"
                                         "Ossi"
                                         "Veikka" |]
                    |> pairJson "Swiss"
                    |> swapJson "Aku" "Ossi"

                Jest
                    .expect(tournament)
                    .toEqual (
                        "{\"rounds\":[{\"number\":1,\"pairings\":[{\"number\":0,\"player1\":\"Ossi\",\"player2\":\"Juha\",\"player1Score\":0,\"player2Score\":0},{\"number\":1,\"player1\":\"Aku\",\"player2\":\"Veikka\",\"player1Score\":0,\"player2Score\":0}],\"status\":\"Pregame\"}],\"players\":[\"Aku\",\"Juha\",\"Ossi\",\"Veikka\"]}"
                    ))
        )
)
