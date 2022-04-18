module TournamentTests

// open System
// open Tournament.Tournament
// open Tournament.PairingGenerator
// open Tournament.Round
// open Tournament.Pairing
// open Tournament.Utils
// open NUnit.Framework

// [<TestFixture>]
// type TestClass() =

//     let unwrap res =
//         match res with
//         | Ok res -> res
//         | Error err -> raise (Exception(err.ToString()))

//     let table number (p1, p2) =
//         let pairing =
//             { Number = 0
//               Player1 = ""
//               Player2 = ""
//               Player1Score = 0
//               Player2Score = 0 }

//         { pairing with
//             Number = number
//             Player1 = p1
//             Player2 = p2 }

//     let result (p1Score, p2Score) pairing =
//         { pairing with
//             Player1Score = p1Score
//             Player2Score = p2Score }

//     let pairings (tournament: Tournament) =
//         tournament.Pairings
//         |> List.map (fun pairing -> (pairing.Player1, pairing.Player2))

//     [<Test>]
//     [<Category("createTournament")>]
//     member this.``creating a tournament with zero or negative rounds returns Error``() =
//         match createTournament 0 with
//         | Ok _ -> failwith "createTournament should not succeed with 0 rounds"
//         | Error msg -> Assert.AreEqual("Tournament should have at least one round", msg)

//     [<Test>]
//     [<Category("createTournament")>]
//     member this.``tournament can be created with a specified number of rounds``() =
//         let tournament = createTournament 5 |> unwrap
//         Assert.AreEqual(5, tournament.Rounds.Length)

//     [<Test>]
//     [<Category("createTournament")>]
//     member this.``initial rounds are numbered, unfinished and have no pairings``() =
//         let tournament = createTournament 2 |> unwrap

//         let expected =
//             [ { Number = 1
//                 Pairings = []
//                 Status = Pregame }
//               { Number = 2
//                 Pairings = []
//                 Status = Pregame } ]

//         Assert.AreEqual(expected, tournament.Rounds)

//     [<Test>]
//     [<Category("addPlayers")>]
//     member this.``players can be added one at a time``() =
//         let tournament =
//             createTournament 1
//             >>= addPlayers [ "Alice" ]
//             |> unwrap

//         CollectionAssert.AreEqual([ "Alice" ], tournament.Players)

//     [<Test>]
//     [<Category("addPlayers")>]
//     member this.``players are added in alphabetical order``() =
//         let tournament =
//             createTournament 1
//             >>= addPlayers [ "Michael" ]
//             >>= addPlayers [ "Alice" ]
//             >>= addPlayers [ "Bob" ]
//             |> unwrap

//         CollectionAssert.AreEqual([ "Alice"; "Bob"; "Michael" ], tournament.Players)

//     [<Test>]
//     [<Category("addPlayers")>]
//     member this.``duplicate players may not be added``() =
//         let tournament =
//             createTournament 1
//             >>= addPlayers [ "Alice" ]
//             >>= addPlayers [ "Alice" ]

//         match tournament with
//         | Ok _ -> failwith "Should not be possible to add duplicate players"
//         | Error err -> Assert.AreEqual("Player Alice already exists", err)

//     [<Test>]
//     [<Category("addPlayers")>]
//     member this.``multiple players can be added at once``() =
//         let tournament =
//             createTournament 1
//             >>= addPlayers [ "Bob"; "Alice" ]
//             |> unwrap

//         CollectionAssert.AreEqual([ "Alice"; "Bob" ], tournament.Players)

//     [<Test>]
//     [<Category("startRound")>]
//     member this.``round can be started``() =
//         let tournament =
//             createTournament 1
//             >>= addPlayers [ "Alice"; "Bob" ]
//             >>= pair Shuffle
//             >>= startRound
//             |> unwrap

//         Assert.AreEqual(true, tournament.Rounds.[0].Status = Ongoing)

//     [<Test>]
//     [<Category("startRound")>]
//     member this.``round cannot be started if round is already ongoing``() =
//         let tournament =
//             createTournament 1
//             >>= addPlayers [ "Alice"; "Bob" ]
//             >>= pair Shuffle
//             >>= startRound
//             >>= startRound

//         match tournament with
//         | Ok _ -> failwith "Did not throw"
//         | Error err -> Assert.AreEqual("Unable to start round 1: round already started", err)

//     [<Test>]
//     [<Category("startRound")>]
//     member this.``round cannot be started without pairings``() =
//         let tournament = createTournament 1 >>= startRound

//         match tournament with
//         | Ok _ -> failwith "Did not throw"
//         | Error err -> Assert.AreEqual("Unable to start round 1: no pairings", err)

//     [<Test>]
//     [<Category("finishRound")>]
//     member this.``round can be marked as finished``() =
//         let t1: Tournament =
//             createTournament 2
//             >>= addPlayers [ "Alice"
//                              "Bob"
//                              "James"
//                              "Michael" ]
//             >>= pair Shuffle
//             >>= startRound // round 1
//             >>= score 0 (15, 5)
//             >>= score 1 (2, 18)
//             >>= finishRound
//             |> unwrap

//         Assert.AreEqual(true, t1.Rounds.[0].Status = Finished)
//         Assert.AreEqual(false, t1.Rounds.[1].Status = Finished)

//         let t2: Tournament =
//             t1
//             |> pair Swiss
//             >>= startRound
//             >>= score 0 (20, 0)
//             >>= score 1 (3, 17)
//             >>= finishRound
//             |> unwrap

//         Assert.AreEqual(true, t2.Rounds.[0].Status = Finished)
//         Assert.AreEqual(true, t2.Rounds.[1].Status = Finished)

//     [<Test>]
//     [<Category("finishRound")>]
//     member this.``current round cannot be finished if round has not started yet``() =
//         let tournament = createTournament 1 >>= finishRound

//         match tournament with
//         | Ok _ -> failwith "Did not throw"
//         | Error err -> Assert.AreEqual(err, "Unable to finish round 1: round not started")

//     [<Test>]
//     [<Category("finishRound")>]
//     member this.``current round cannot be finished if tournament is already finished``() =
//         let tournament =
//             createTournament 1
//             >>= addPlayers [ "Alice"; "Bob" ]
//             >>= pair Shuffle
//             >>= startRound
//             >>= score 0 (10, 10)
//             >>= finishRound
//             >>= finishRound

//         match tournament with
//         | Ok _ -> failwith "Did not throw"
//         | Error err -> Assert.AreEqual(err, "Tournament already finished")

//     [<Test>]
//     [<Category("finishRound")>]
//     member this.``current round cannot be finished if unscored pairings exist``() =
//         let tournament =
//             createTournament 1
//             >>= addPlayers [ "Alice"; "Bob" ]
//             >>= pair Shuffle
//             >>= startRound
//             >>= finishRound

//         match tournament with
//         | Ok _ -> failwith "Did not throw"
//         | Error err -> Assert.AreEqual(err, "Unable to finish round 1: unscored pairings exist")

//     [<Test>]
//     [<Category("pair")>]
//     member this.``pairings for current round are determined by the order of pairing algorithm``() =
//         let tournament =
//             createTournament 1
//             >>= addPlayers [ "Alice"
//                              "Bob"
//                              "James"
//                              "Michael" ]
//             >>= pair Swiss
//             |> unwrap

//         let pairings = tournament.Rounds.[0].Pairings
//         Assert.AreEqual("Alice", pairings.[0].Player1)
//         Assert.AreEqual("Bob", pairings.[0].Player2)
//         Assert.AreEqual("James", pairings.[1].Player1)
//         Assert.AreEqual("Michael", pairings.[1].Player2)

//     [<Test>]
//     [<Category("pair")>]
//     member this.``on odd number of players, BYE is added to player list for pairings``() =
//         let tournament =
//             createTournament 1
//             >>= addPlayers [ "Alice" ]
//             >>= pair Swiss
//             |> unwrap

//         Assert.AreEqual("Alice", tournament.Rounds.[0].Pairings.[0].Player1)
//         Assert.AreEqual("BYE", tournament.Rounds.[0].Pairings.[0].Player2)

//     [<Test>]
//     [<Category("score")>]
//     member this.``score can be set for pairing by table number``() =
//         let unscored =
//             [ { Number = 1
//                 Status = Ongoing
//                 Pairings =
//                   [ table 123 ("Alice", "Bob")
//                     table 456 ("James", "Michael") ] } ]

//         let tournament =
//             { (createTournament 1 |> unwrap) with Rounds = unscored }
//             |> score 456 (13, 8)
//             |> unwrap

//         let round = tournament.Rounds.[0]
//         Assert.AreEqual(unscored.[0].Pairings.[0], round.Pairings.[0])
//         Assert.AreEqual(13, round.Pairings.[1].Player1Score)
//         Assert.AreEqual(8, round.Pairings.[1].Player2Score)

//     [<Test>]
//     [<Category("score")>]
//     member this.``score is set for current round only``() =
//         let unscored =
//             [ { Number = 1
//                 Status = Finished
//                 Pairings = [ table 123 ("Alice", "Bob") ] }
//               { Number = 2
//                 Status = Ongoing
//                 Pairings = [ table 123 ("Bob", "Alice") ] } ]

//         let tournament =
//             { (createTournament 1 |> unwrap) with Rounds = unscored }
//             |> score 123 (15, 5)
//             |> unwrap

//         Assert.AreEqual(unscored.[0], tournament.Rounds.[0])
//         Assert.AreEqual(15, tournament.Rounds.[1].Pairings.[0].Player1Score)
//         Assert.AreEqual(5, tournament.Rounds.[1].Pairings.[0].Player2Score)

//     [<Test>]
//     [<Category("score")>]
//     member this.``score returns Error if specified pairing cannot be found``() =
//         let tournament =
//             createTournament 1
//             >>= addPlayers [ "Alice"; "Bob" ]
//             >>= pair Shuffle
//             >>= startRound
//             >>= score 123 (11, 9)

//         match tournament with
//         | Ok _ -> failwith "Did not throw"
//         | Error err -> Assert.AreEqual("Match 123 not found!", err)

//     [<Test>]
//     [<Category("score")>]
//     member this.``score returns Error if round has already been marked as finished``() =
//         match createTournament 1
//               >>= addPlayers [ "Alice"; "Bob" ]
//               >>= pair Shuffle
//               >>= startRound
//               >>= score 0 (10, 10)
//               >>= finishRound
//               >>= score 0 (11, 9)
//             with
//         | Ok _ -> failwith "Did not throw"
//         | Error err -> Assert.AreEqual("Tournament already finished", err)

//     [<Test>]
//     [<Category("pairings")>]
//     member this.``pairings display pairings of current round or last round if tournament is finished``() =
//         let tournament =
//             createTournament 1
//             >>= addPlayers [ "Alice"
//                              "Bob"
//                              "James"
//                              "Michael" ]
//             >>= pair Swiss
//             >>= startRound
//             |> unwrap

//         CollectionAssert.AreEqual(
//             [ ("Alice", "Bob")
//               ("James", "Michael") ],
//             pairings tournament
//         )

//         let finished =
//             tournament
//             |> score 0 (16, 4)
//             >>= score 1 (1, 19)
//             >>= finishRound
//             |> unwrap

//         CollectionAssert.AreEqual(pairings tournament, pairings finished)

//     [<Test>]
//     [<Category("standings")>]
//     member this.``round standings are listed in map displaying names and scores``() =
//         let round =
//             { Number = 1
//               Status = Finished
//               Pairings =
//                 [ { Number = 1
//                     Player1 = "Alice"
//                     Player2 = "Bob"
//                     Player1Score = 4
//                     Player2Score = 16 }
//                   { Number = 2
//                     Player1 = "James"
//                     Player2 = "Michael"
//                     Player1Score = 15
//                     Player2Score = 5 } ] }

//         CollectionAssert.AreEqual(
//             Map.ofList (
//                 [ ("Bob", 16)
//                   ("James", 15)
//                   ("Michael", 5)
//                   ("Alice", 4) ]
//             ),
//             round.Standings
//         )

//     [<Test>]
//     [<Category("standings")>]
//     member this.``total standings are listed in map with scores from all rounds``() =
//         let rounds =
//             [ { Number = 1
//                 Status = Finished
//                 Pairings =
//                   [ table 1 ("Alice", "Bob") |> result (12, 8)
//                     table 2 ("James", "Michael") |> result (1, 19) ] }
//               { Number = 2
//                 Status = Finished
//                 Pairings =
//                   [ table 1 ("Michael", "Alice") |> result (13, 7)
//                     table 2 ("Bob", "James") |> result (10, 10) ] } ]

//         let tournament = { (createTournament 2 |> unwrap) with Rounds = rounds }

//         CollectionAssert.AreEqual(
//             [ ("Michael", 32)
//               ("Alice", 19)
//               ("Bob", 18)
//               ("James", 11) ],
//             tournament.Standings
//         )

//     [<Test>]
//     [<Category("standings")>]
//     member this.``total standings are equal to player list if no rounds have been played``() =
//         let round =
//             { Number = 1
//               Status = Pregame
//               Pairings = [] }

//         let tournament =
//             createTournament 1
//             >>= addPlayers [ "Alice"
//                              "Bob"
//                              "James"
//                              "Michael" ]
//             |> unwrap

//         CollectionAssert.AreEqual(
//             [ ("Alice", 0)
//               ("Bob", 0)
//               ("James", 0)
//               ("Michael", 0) ],
//             tournament.Standings
//         )

//     [<Test>]
//     [<Category("swap")>]
//     member this.``swap changes places of two players in current round``() =
//         let tournament =
//             createTournament 1
//             >>= addPlayers [ "Alice"
//                              "Bob"
//                              "James"
//                              "Michael" ]
//             >>= pair Swiss
//             >>= swap "Bob" "Michael"
//             |> unwrap

//         CollectionAssert.AreEqual(
//             [ ("Alice", "Michael")
//               ("James", "Bob") ],
//             pairings tournament
//         )

//     [<Test>]
//     [<Category("swap")>]
//     member this.``swap changes places even if they are already against each other``() =
//         let rounds =
//             [ { Number = 1
//                 Status = Pregame
//                 Pairings = [ table 1 ("Alice", "Bob") ] } ]

//         let tournament =
//             { (createTournament 1 |> unwrap) with Rounds = rounds }
//             |> swap "Alice" "Bob"
//             |> unwrap

//         Assert.AreEqual(table 1 ("Bob", "Alice"), tournament.Rounds.[0].Pairings.[0])

//     [<Test>]
//     [<Category("swap")>]
//     member this.``swap returns Error if either player is not found in pairings list``() =
//         let rounds =
//             [ { Number = 1
//                 Status = Pregame
//                 Pairings =
//                   [ table 1 ("Alice", "Bob")
//                     table 2 ("James", "Michael") ] } ]

//         match ({ (createTournament 1 |> unwrap) with Rounds = rounds }
//                |> swap "Alice" "Nyarlathotep")
//             with
//         | Ok _ -> failwith "Trying to swap nonexistent players should return Error"
//         | Error err -> Assert.AreEqual("Player Nyarlathotep not found", err)

//     [<Test>]
//     [<Category("swap")>]
//     member this.``swap returns Error if either player's initial round has been scored``() =
//         let rounds =
//             [ { Number = 1
//                 Status = Pregame
//                 Pairings =
//                   [ table 1 ("Alice", "Bob") |> result (19, 1)
//                     table 2 ("James", "Michael") ] } ]

//         match ({ (createTournament 1 |> unwrap) with Rounds = rounds }
//                |> swap "Alice" "Michael")
//             with
//         | Ok _ -> failwith "Trying to swap players should return Error if either player's round is scored"
//         | Error err -> Assert.AreEqual("Can't swap players if either player's round has already been scored!", err)

//     [<Test>]
//     [<Category("json")>]
//     member this.``createTournamentJson returns empty tournament as json``() =
//         let tournament = createTournamentJson 1

//         Assert.AreEqual(
//             "{\"rounds\":[{\"number\":1,\"pairings\":[],\"status\":\"Pregame\"}],\"players\":[]}",
//             tournament
//         )

//     [<Test>]
//     [<Category("json")>]
//     member this.``addPlayersJson adds players correctly to tournament``() =
//         let t = createTournamentJson 1

//         // add 2 players
//         let tournamentWithPlayers = addPlayersJson [ "Alice"; "Bob" ] t

//         Assert.AreEqual(
//             "{\"rounds\":[{\"number\":1,\"pairings\":[],\"status\":\"Pregame\"}],\"players\":[\"Alice\",\"Bob\"]}",
//             tournamentWithPlayers
//         )

//         // add 1 more player
//         let tournamentWithMorePlayers = addPlayersJson [ "Jack" ] tournamentWithPlayers

//         Assert.AreEqual(
//             "{\"rounds\":[{\"number\":1,\"pairings\":[],\"status\":\"Pregame\"}],\"players\":[\"Alice\",\"Bob\",\"Jack\"]}",
//             tournamentWithMorePlayers
//         )

//     [<Test>]
//     member this.``tournament can be run successfully from start to finish ensuring unique pairings each round``() =

//         // ROUND 1 PAIRINGS
//         let round1 =
//             createTournament 4
//             >>= addPlayers [ "Alice"
//                              "Bob"
//                              "Jack"
//                              "James"
//                              "Lily"
//                              "Michael" ]
//             >>= pair Swiss
//             >>= startRound
//             |> unwrap

//         CollectionAssert.AreEqual(
//             [ ("Alice", "Bob")
//               ("Jack", "James")
//               ("Lily", "Michael") ],
//             pairings round1
//         )

//         // ROUND 1 SCORES
//         let round1Finished =
//             round1
//             |> score 0 (15, 5) // Alice 15, Bob 5
//             >>= score 1 (11, 9) // Jack 11, James 9
//             >>= score 2 (7, 13) // Lily 7, Michael 13
//             >>= finishRound
//             |> unwrap

//         CollectionAssert.AreEqual(
//             [ ("Alice", 15)
//               ("Michael", 13)
//               ("Jack", 11)
//               ("James", 9)
//               ("Lily", 7)
//               ("Bob", 5) ],
//             round1Finished.Standings
//         )

//         // ROUND 2 PAIRINGS
//         let round2 =
//             round1Finished
//             |> pair Swiss
//             >>= startRound
//             |> unwrap

//         CollectionAssert.AreEqual(
//             [ ("Alice", "Michael")
//               ("Jack", "Lily") // Jack has already played James -> Lily is next eligible opponent
//               ("James", "Bob") ],
//             pairings round2
//         )

//         // ROUND 2 RESULTS
//         let round2Finished =
//             round2
//             |> score 0 (20, 0) // Alice 20, Michael 0
//             >>= score 1 (4, 16) // Jack 4, Lily 16
//             >>= score 2 (9, 11) // James 9, Bob 11
//             >>= finishRound
//             |> unwrap

//         CollectionAssert.AreEqual(
//             [ ("Alice", 35)
//               ("Lily", 23)
//               ("James", 18)
//               ("Bob", 16)
//               ("Jack", 15)
//               ("Michael", 13) ],
//             round2Finished.Standings
//         )

//         // ROUND 3 PAIRINGS
//         let round3 =
//             round2Finished
//             |> pair Swiss
//             >>= startRound
//             |> unwrap

//         CollectionAssert.AreEqual(
//             [ ("Alice", "Lily")
//               ("James", "Michael") // James has already played Jack & Bob -> Michael is the only eligible opponent
//               ("Bob", "Jack") ],
//             pairings round3
//         )

//         // ROUND 3 RESULTS
//         let round3Finished =
//             round3
//             |> score 0 (20, 0) // Alice 20, Lily 0
//             >>= score 1 (9, 11) // James 9, Michael 11
//             >>= score 2 (10, 10) // Bob 10, Jack 10
//             >>= finishRound
//             |> unwrap

//         CollectionAssert.AreEqual(
//             [ ("Alice", 55)
//               ("James", 27)
//               ("Bob", 26)
//               ("Jack", 25)
//               ("Michael", 24)
//               ("Lily", 23) ],
//             round3Finished.Standings
//         )

//         // ROUND 4 PAIRINGS
//         let round4 =
//             round3Finished
//             |> pair Swiss
//             >>= startRound
//             |> unwrap

//         CollectionAssert.AreEqual(
//             [ ("Alice", "James")
//               ("Bob", "Lily") // Bob has already played Jack and the next opponent, Michael, would leave Jack & Lily for last, who have already played -> Lily is the only eligible opponent
//               ("Jack", "Michael") ],
//             pairings round4
//         )

//         // ROUND 4 RESULTS
//         let round4Finished =
//             round4
//             |> score 0 (3, 17) // Alice 3, James 17
//             >>= score 1 (9, 11) // Bob 9, Lily 11
//             >>= score 2 (15, 5) // Jack 15, Michael 5
//             >>= finishRound
//             |> unwrap

//         CollectionAssert.AreEqual(
//             [ ("Alice", 58)
//               ("James", 44)
//               ("Jack", 40)
//               ("Bob", 35)
//               ("Lily", 34)
//               ("Michael", 29) ],
//             round4Finished.Standings
//         )
