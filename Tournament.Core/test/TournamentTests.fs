module TournamentTests

open Tournament.Tournament
open Tournament.PairingGenerator
open Tournament.Round
open Tournament.Pairing
open Tournament.Utils
open Tournament.Player
open NUnit.Framework

[<TestFixture>]
type TestClass() =

    let table number (p1, p2) =
        { Number = number
          Player1 = p1, Score.Empty
          Player2 = p2, Score.Empty }

    let result (p1Score: int, p2Score: int) pairing =
        { pairing with
            Player1 = fst pairing.Player1, Score.Of p1Score
            Player2 = fst pairing.Player2, Score.Of p2Score }

    let pairings (tournament: Tournament) =
        tournament.Pairings
        |> List.map (fun pairing -> (fst pairing.Player1, fst pairing.Player2))

    let names players = players |> List.map (fun p -> p.Name)

    [<Test>]
    [<Category("Tournament.Create")>]
    member this.``creating a tournament with zero or negative rounds returns Error``() =
        match Tournament.Create 0 with
        | Ok _ -> failwith "Tournament.Create should not succeed with 0 rounds"
        | Error msg -> Assert.AreEqual("Tournament should have at least one round", msg)

    [<Test>]
    [<Category("Tournament.Create")>]
    member this.``tournament can be created with a specified number of rounds``() =
        let tournament = Tournament.Create 5 |> unwrap
        Assert.AreEqual(5, tournament.Rounds.Length)

    [<Test>]
    [<Category("Tournament.Create")>]
    member this.``initial rounds are numbered, unfinished and have no pairings``() =
        let tournament = Tournament.Create 2 |> unwrap

        let expected =
            [ { Number = 1
                Pairings = []
                Status = Pregame }
              { Number = 2
                Pairings = []
                Status = Pregame } ]

        Assert.AreEqual(expected, tournament.Rounds)

    [<Test>]
    [<Category("Tournament.Create")>]
    member this.``tournament can be created with initial player list``() =
        let tournament =
            Tournament.Create(1, [ "Alice"; "Bob"; "Michael"; "James" ])
            |> unwrap

        Assert.AreEqual(
            [ Player.From "Alice"
              Player.From "Bob"
              Player.From "Michael"
              Player.From "James" ],
            tournament.Players
        )

    [<Test>]
    [<Category("Tournament.Create")>]
    member this.``tournament cannot be created from inital player list if there are duplicate players in list``() =
        let tournament = Tournament.Create(1, [ "Alice"; "Alice" ])

        match tournament with
        | Ok _ -> failwith "Should not be possible to add duplicate players"
        | Error err -> Assert.AreEqual("All player names must be unique", err)

    [<Test>]
    [<Category("Tournament.Create")>]
    member this.``tournament cannot be created from inital player list if there is empty player name in list``() =
        let tournament = Tournament.Create(1, [ "Alice"; "" ])

        match tournament with
        | Ok _ -> failwith "Should not be possible to add duplicate players"
        | Error err -> Assert.AreEqual("Empty name is not allowed", err)

    [<Test>]
    [<Category("addPlayers")>]
    member this.``players can be added one at a time``() =
        let tournament =
            Tournament.Create 1
            >>= addPlayers [ Player.From "Alice" ]
            |> unwrap

        CollectionAssert.AreEqual([ "Alice" ], names tournament.Players)

    [<Test>]
    [<Category("addPlayers")>]
    member this.``players are added in alphabetical order``() =
        let tournament =
            Tournament.Create 1
            >>= addPlayers [ Player.From "Michael" ]
            >>= addPlayers [ Player.From "Alice" ]
            >>= addPlayers [ Player.From "Bob" ]
            |> unwrap

        CollectionAssert.AreEqual([ "Alice"; "Bob"; "Michael" ], names tournament.Players)

    [<Test>]
    [<Category("addPlayers")>]
    member this.``duplicate players may not be added``() =
        let tournament =
            Tournament.Create 1
            >>= addPlayers [ Player.From "Alice" ]
            >>= addPlayers [ Player.From "Alice" ]

        match tournament with
        | Ok _ -> failwith "Should not be possible to add duplicate players"
        | Error err -> Assert.AreEqual("Player Alice already exists", err)


    [<Test>]
    [<Category("addPlayers")>]
    member this.``multiple players with duplicate names cannot be added``() =
        let tournament =
            Tournament.Create 1
            >>= addPlayers [ Player.From "Alice"
                             Player.From "Alice" ]

        match tournament with
        | Ok _ -> failwith "Should not be possible to add duplicate players"
        | Error err -> Assert.AreEqual("Player Alice already exists", err)

    [<Test>]
    [<Category("addPlayers")>]
    member this.``multiple players can be added at once``() =
        let tournament =
            Tournament.Create 1
            >>= addPlayers [ Player.From "Bob"
                             Player.From "Alice" ]
            |> unwrap

        CollectionAssert.AreEqual([ "Alice"; "Bob" ], names tournament.Players)

    [<Test>]
    [<Category("addPlayers")>]
    member this.``players with empty name cannot be added``() =
        let tournament =
            Tournament.Create 1
            >>= addPlayers [ Player.From "" ]

        match tournament with
        | Ok _ -> failwith "Should not be possible to add players with empty name"
        | Error err -> Assert.AreEqual("Players with empty name are not allowed", err)

    [<Test>]
    [<Category("startRound")>]
    member this.``round can be started``() =
        let tournament =
            Tournament.Create(1, [ "Alice"; "Bob" ])
            >>= pair Shuffle
            >>= startRound
            |> unwrap

        Assert.AreEqual(true, tournament.Rounds.[0].Status = Ongoing)

    [<Test>]
    [<Category("startRound")>]
    member this.``round cannot be started if round is already ongoing``() =
        let tournament =
            Tournament.Create(1, [ "Alice"; "Bob" ])
            >>= pair Shuffle
            >>= startRound
            >>= startRound

        match tournament with
        | Ok _ -> failwith "Did not throw"
        | Error err -> Assert.AreEqual("Unable to start round 1: round already started", err)

    [<Test>]
    [<Category("startRound")>]
    member this.``round cannot be started without pairings``() =
        let tournament = Tournament.Create 1 >>= startRound

        match tournament with
        | Ok _ -> failwith "Did not throw"
        | Error err -> Assert.AreEqual("Unable to start round 1: no pairings", err)

    [<Test>]
    [<Category("finishRound")>]
    member this.``round can be marked as finished``() =
        let t1: Tournament =
            Tournament.Create(2, [ "Alice"; "Bob"; "James"; "Michael" ])
            >>= pair Shuffle
            >>= startRound // round 1
            >>= score 1 (Score.Of 15, Score.Of 5)
            >>= score 2 (Score.Of 2, Score.Of 18)
            >>= finishRound
            |> unwrap

        Assert.AreEqual(true, t1.Rounds.[0].Status = Finished)
        Assert.AreEqual(false, t1.Rounds.[1].Status = Finished)

        let t2: Tournament =
            t1
            |> pair Swiss
            >>= startRound
            >>= score 1 (Score.Of 20, Score.Of 0)
            >>= score 2 (Score.Of 3, Score.Of 17)
            >>= finishRound
            |> unwrap

        Assert.AreEqual(true, t2.Rounds.[0].Status = Finished)
        Assert.AreEqual(true, t2.Rounds.[1].Status = Finished)

    [<Test>]
    [<Category("finishRound")>]
    member this.``current round cannot be finished if round has not started yet``() =
        let tournament = Tournament.Create 1 >>= finishRound

        match tournament with
        | Ok _ -> failwith "Did not throw"
        | Error err -> Assert.AreEqual(err, "Unable to finish round 1: round not started")

    [<Test>]
    [<Category("finishRound")>]
    member this.``current round cannot be finished if tournament is already finished``() =
        let tournament =
            Tournament.Create(1, [ "Alice"; "Bob" ])
            >>= pair Shuffle
            >>= startRound
            >>= score 1 (Score.Of 10, Score.Of 10)
            >>= finishRound
            >>= finishRound

        match tournament with
        | Ok _ -> failwith "Did not throw"
        | Error err -> Assert.AreEqual(err, "Tournament already finished")

    [<Test>]
    [<Category("finishRound")>]
    member this.``current round cannot be finished if unscored pairings exist``() =
        let tournament =
            Tournament.Create(1, [ "Alice"; "Bob" ])
            >>= pair Shuffle
            >>= startRound
            >>= finishRound

        match tournament with
        | Ok _ -> failwith "Did not throw"
        | Error err -> Assert.AreEqual(err, "Unable to finish round 1: unscored pairings exist")

    [<Test>]
    [<Category("pair")>]
    member this.``pairings for current round are determined by the order of pairing algorithm``() =
        let tournament =
            Tournament.Create(1, [ "Alice"; "Bob"; "James"; "Michael" ])
            >>= pair Swiss
            |> unwrap

        let pairings = tournament.Rounds.[0].Pairings
        Assert.AreEqual("Alice", fst pairings.[0].Player1)
        Assert.AreEqual("Bob", fst pairings.[0].Player2)
        Assert.AreEqual("James", fst pairings.[1].Player1)
        Assert.AreEqual("Michael", fst pairings.[1].Player2)

    [<Test>]
    [<Category("pair")>]
    member this.``on odd number of players, BYE is added to player list for pairings``() =
        let tournament =
            Tournament.Create(1, [ "Alice" ])
            >>= pair Swiss
            |> unwrap

        Assert.AreEqual("Alice", fst tournament.Rounds.[0].Pairings.[0].Player1)
        Assert.AreEqual("BYE", fst tournament.Rounds.[0].Pairings.[0].Player2)

    [<Test>]
    [<Category("score")>]
    member this.``score can be set for pairing by table number``() =
        let unscored =
            [ { Number = 1
                Status = Ongoing
                Pairings =
                  [ table 123 ("Alice", "Bob")
                    table 456 ("James", "Michael") ] } ]

        let tournament =
            { (Tournament.Create 1 |> unwrap) with Rounds = unscored }
            |> score 456 (Score.Of 13, Score.Of 8)
            |> unwrap

        let round = tournament.Rounds.[0]
        Assert.AreEqual(unscored.[0].Pairings.[0], round.Pairings.[0])
        Assert.AreEqual(13, (snd round.Pairings.[1].Player1).Primary)
        Assert.AreEqual(8, (snd round.Pairings.[1].Player2).Primary)

    [<Test>]
    [<Category("score")>]
    member this.``score is set for current round only``() =
        let unscored =
            [ { Number = 1
                Status = Finished
                Pairings = [ table 123 ("Alice", "Bob") ] }
              { Number = 2
                Status = Ongoing
                Pairings = [ table 123 ("Bob", "Alice") ] } ]

        let tournament =
            { (Tournament.Create 1 |> unwrap) with Rounds = unscored }
            |> score 123 (Score.Of 15, Score.Of 5)
            |> unwrap

        Assert.AreEqual(unscored.[0], tournament.Rounds.[0])

        Assert.AreEqual(
            15,
            (snd tournament.Rounds.[1].Pairings.[0].Player1)
                .Primary
        )

        Assert.AreEqual(
            5,
            (snd tournament.Rounds.[1].Pairings.[0].Player2)
                .Primary
        )

    [<Test>]
    [<Category("score")>]
    member this.``score returns Error if specified pairing cannot be found``() =
        let tournament =
            Tournament.Create(1, [ "Alice"; "Bob" ])
            >>= pair Shuffle
            >>= startRound
            >>= score 123 (Score.Of 11, Score.Of 9)

        match tournament with
        | Ok _ -> failwith "Did not throw"
        | Error err -> Assert.AreEqual("Match 123 not found!", err)

    [<Test>]
    [<Category("score")>]
    member this.``score returns Error if round has already been marked as finished``() =
        match Tournament.Create(1, [ "Alice"; "Bob" ])
              >>= pair Shuffle
              >>= startRound
              >>= score 1 (Score.Of 10, Score.Of 10)
              >>= finishRound
              >>= score 1 (Score.Of 11, Score.Of 9)
            with
        | Ok _ -> failwith "Did not throw"
        | Error err -> Assert.AreEqual("Tournament already finished", err)

    [<Test>]
    [<Category("pairings")>]
    member this.``pairings display pairings of current round or last round if tournament is finished``() =
        let tournament =
            Tournament.Create(1, [ "Alice"; "Bob"; "James"; "Michael" ])
            >>= pair Swiss
            >>= startRound
            |> unwrap

        CollectionAssert.AreEqual(
            [ ("Alice", "Bob")
              ("James", "Michael") ],
            pairings tournament
        )

        let finished =
            tournament
            |> score 1 (Score.Of 16, Score.Of 4)
            >>= score 2 (Score.Of 1, Score.Of 19)
            >>= finishRound
            |> unwrap

        CollectionAssert.AreEqual(pairings tournament, pairings finished)

    [<Test>]
    [<Category("standings")>]
    member this.``round standings is a of players ordered by score``() =
        let round =
            { Number = 1
              Status = Finished
              Pairings =
                [ { Number = 1
                    Player1 = "Alice", (Score.Of 4)
                    Player2 = "Bob", (Score.Of 16) }
                  { Number = 2
                    Player1 = "James", (Score.Of 15)
                    Player2 = "Michael", (Score.Of 5) } ] }

        CollectionAssert.AreEqual(
            [ "Bob", (Score.Of 16)
              "James", (Score.Of 15)
              "Michael", (Score.Of 5)
              "Alice", (Score.Of 4) ],
            round.Standings
        )


    [<Test>]
    [<Category("standings")>]
    member this.``round standings uses secondary score as tiebreaker``() =
        let round =
            { Number = 1
              Status = Finished
              Pairings =
                [ { Number = 1
                    Player1 = "Alice", { Primary = 10; Secondary = 1 }
                    Player2 = "Bob", { Primary = 10; Secondary = 4 } }
                  { Number = 2
                    Player1 = "James", { Primary = 10; Secondary = 2 }
                    Player2 = "Michael", { Primary = 10; Secondary = 3 } } ] }

        CollectionAssert.AreEqual(
            [ "Bob", { Primary = 10; Secondary = 4 }
              "Michael", { Primary = 10; Secondary = 3 }
              "James", { Primary = 10; Secondary = 2 }
              "Alice", { Primary = 10; Secondary = 1 } ],
            round.Standings
        )


    [<Test>]
    [<Category("standings")>]
    member this.``tournament standings counts scores from all rounds``() =
        let rounds =
            [ { Number = 1
                Status = Finished
                Pairings =
                  [ table 1 ("Alice", "Bob") |> result (12, 8)
                    table 2 ("James", "Michael") |> result (1, 19) ] }
              { Number = 2
                Status = Finished
                Pairings =
                  [ table 1 ("Michael", "Alice") |> result (13, 7)
                    table 2 ("Bob", "James") |> result (10, 10) ] } ]

        let tournament = { (Tournament.Create 2 |> unwrap) with Rounds = rounds }

        CollectionAssert.AreEqual(
            [ ("Michael", Score.Of 32)
              ("Alice", Score.Of 19)
              ("Bob", Score.Of 18)
              ("James", Score.Of 11) ],
            tournament.Standings()
        )

    [<Test>]
    [<Category("standings")>]
    member this.``tournament standings uses secondary score as tiebreaker from all rounds``() =
        let tournament =
            Tournament.Create(2, [ "Alice"; "Bob"; "James"; "Michael" ])
            >>= pair Swiss
            >>= startRound
            >>= score 1 ({ Primary = 10; Secondary = 1 }, { Primary = 10; Secondary = 4 }) // Alice 10 (1), Bob 10 (4)
            >>= score 2 ({ Primary = 10; Secondary = 2 }, { Primary = 10; Secondary = 3 }) // James 10 (2), Michael 10 (3)
            >>= finishRound
            >>= pair Swiss
            >>= startRound
            >>= score 1 ({ Primary = 10; Secondary = 4 }, { Primary = 10; Secondary = 2 }) // Bob 10 (4), Michael 10 (2)
            >>= score 2 ({ Primary = 10; Secondary = 3 }, { Primary = 10; Secondary = 1 }) // James 10 (3), Alice 10 (1)
            |> unwrap

        CollectionAssert.AreEqual(
            [ "Bob", { Primary = 20; Secondary = 8 }
              "James", { Primary = 20; Secondary = 5 }
              "Michael", { Primary = 20; Secondary = 5 }
              "Alice", { Primary = 20; Secondary = 2 } ],
            tournament.Standings()
        )

    [<Test>]
    [<Category("standings")>]
    member this.``tournament standings are equal to player list if no rounds have been played``() =
        let tournament =
            Tournament.Create(1, [ "Alice"; "Bob"; "James"; "Michael" ])
            |> unwrap

        CollectionAssert.AreEqual(
            [ ("Alice", Score.Of 0)
              ("Bob", Score.Of 0)
              ("James", Score.Of 0)
              ("Michael", Score.Of 0) ],
            tournament.Standings()
        )

    [<Test>]
    [<Category("swap")>]
    member this.``swap changes places of two players in current round``() =
        let tournament =
            Tournament.Create(1, [ "Alice"; "Bob"; "James"; "Michael" ])
            >>= pair Swiss
            >>= swap "Bob" "Michael"
            |> unwrap

        CollectionAssert.AreEqual(
            [ ("Alice", "Michael")
              ("James", "Bob") ],
            pairings tournament
        )

    [<Test>]
    [<Category("swap")>]
    member this.``swap changes places even if they are already against each other``() =
        let rounds =
            [ { Number = 1
                Status = Pregame
                Pairings = [ table 1 ("Alice", "Bob") ] } ]

        let tournament =
            { (Tournament.Create 1 |> unwrap) with Rounds = rounds }
            |> swap "Alice" "Bob"
            |> unwrap

        Assert.AreEqual(table 1 ("Bob", "Alice"), tournament.Rounds.[0].Pairings.[0])

    [<Test>]
    [<Category("swap")>]
    member this.``swap returns Error if either player is not found in pairings list``() =
        let rounds =
            [ { Number = 1
                Status = Pregame
                Pairings =
                  [ table 1 ("Alice", "Bob")
                    table 2 ("James", "Michael") ] } ]

        match ({ (Tournament.Create 1 |> unwrap) with Rounds = rounds }
               |> swap "Alice" "Nyarlathotep")
            with
        | Ok _ -> failwith "Trying to swap nonexistent players should return Error"
        | Error err -> Assert.AreEqual("Player Nyarlathotep not found", err)

    [<Test>]
    [<Category("swap")>]
    member this.``swap returns Error if round has already started``() =
        match Tournament.Create(1, [ "Alice"; "Bob"; "James"; "Michael" ])
              >>= pair Swiss
              >>= startRound
              >>= swap "Alice" "Michael"
            with
        | Ok _ -> failwith "Trying to swap players should return Error if round has already started"
        | Error err -> Assert.AreEqual("Can't swap players: round already started!", err)

    [<Test>]
    [<Category("bonus")>]
    member this.``bonus returns Error if player cannot be found``() =
        match
            Tournament.Create(1, [ "Alice"; "Bob" ])
            >>= bonus ("Hastur", 5)
            with
        | Ok _ -> failwith "Trying to add bonus to nonexistent player should return Error"
        | Error err -> Assert.AreEqual("Player Hastur not found", err)

    [<Test>]
    [<Category("bonus")>]
    member this.``bonus sets bonus score for a player``() =
        let tournament =
            Tournament.Create(1, [ "Alice"; "Bob" ])
            >>= bonus ("Alice", 5)
            |> unwrap

        Assert.AreEqual(tournament.Players.[0].BonusScore, 5)

    [<Test>]
    member this.``tournament can be run successfully from start to finish ensuring unique pairings each round``() =
        // ROUND 1 PAIRINGS
        let round1 =
            Tournament.Create(4)
            >>= addPlayers [ Player.From "Alice"
                             Player.From "Bob"
                             Player.From "Jack"
                             Player.From "James"
                             Player.From "Lily"
                             Player.From "Michael" ]
            >>= pair Swiss
            >>= startRound
            |> unwrap

        CollectionAssert.AreEqual(
            [ ("Alice", "Bob")
              ("Jack", "James")
              ("Lily", "Michael") ],
            pairings round1
        )

        // ROUND 1 SCORES
        let round1Finished =
            round1
            |> score 1 (Score.Of 15, Score.Of 5) // Alice 15, Bob 5
            >>= score 2 (Score.Of 11, Score.Of 9) // Jack 11, James 9
            >>= score 3 (Score.Of 7, Score.Of 13) // Lily 7, Michael 13
            >>= finishRound
            |> unwrap

        CollectionAssert.AreEqual(
            [ ("Alice", Score.Of 15)
              ("Michael", Score.Of 13)
              ("Jack", Score.Of 11)
              ("James", Score.Of 9)
              ("Lily", Score.Of 7)
              ("Bob", Score.Of 5) ],
            round1Finished.Standings()
        )

        // ROUND 2 PAIRINGS
        let round2 =
            round1Finished
            |> pair Swiss
            >>= startRound
            |> unwrap

        CollectionAssert.AreEqual(
            [ ("Alice", "Michael")
              ("Jack", "Lily") // Jack has already played James -> Lily is next eligible opponent
              ("James", "Bob") ],
            pairings round2
        )

        // ROUND 2 RESULTS
        let round2Finished =
            round2
            |> score 1 (Score.Of 20, Score.Of 0) // Alice 20, Michael 0
            >>= score 2 (Score.Of 4, Score.Of 16) // Jack 4, Lily 16
            >>= score 3 (Score.Of 9, Score.Of 11) // James 9, Bob 11
            >>= finishRound
            |> unwrap

        CollectionAssert.AreEqual(
            [ ("Alice", Score.Of 35)
              ("Lily", Score.Of 23)
              ("James", Score.Of 18)
              ("Bob", Score.Of 16)
              ("Jack", Score.Of 15)
              ("Michael", Score.Of 13) ],
            round2Finished.Standings()
        )

        // ROUND 3 PAIRINGS
        let round3 =
            round2Finished
            |> pair Swiss
            >>= startRound
            |> unwrap

        CollectionAssert.AreEqual(
            [ ("Alice", "Lily")
              ("James", "Michael") // James has already played Jack & Bob -> Michael is the only eligible opponent
              ("Bob", "Jack") ],
            pairings round3
        )

        // ROUND 3 RESULTS
        let round3Finished =
            round3
            |> score 1 (Score.Of 20, Score.Of 0) // Alice 20, Lily 0
            >>= score 2 (Score.Of 9, Score.Of 11) // James 9, Michael 11
            >>= score 3 (Score.Of 10, Score.Of 10) // Bob 10, Jack 10
            >>= finishRound
            |> unwrap

        CollectionAssert.AreEqual(
            [ ("Alice", Score.Of 55)
              ("James", Score.Of 27)
              ("Bob", Score.Of 26)
              ("Jack", Score.Of 25)
              ("Michael", Score.Of 24)
              ("Lily", Score.Of 23) ],
            round3Finished.Standings()
        )

        // ROUND 4 PAIRINGS
        let round4 =
            round3Finished
            |> pair Swiss
            >>= startRound
            |> unwrap

        CollectionAssert.AreEqual(
            [ ("Alice", "James")
              ("Bob", "Lily") // Bob has already played Jack and the next opponent, Michael, would leave Jack & Lily for last, who have already played -> Lily is the only eligible opponent
              ("Jack", "Michael") ],
            pairings round4
        )

        // ROUND 4 RESULTS
        let round4Finished =
            round4
            |> score 1 (Score.Of 3, Score.Of 17) // Alice 3, James 17
            >>= score 2 (Score.Of 9, Score.Of 11) // Bob 9, Lily 11
            >>= score 3 (Score.Of 15, Score.Of 5) // Jack 15, Michael 5
            >>= finishRound
            |> unwrap

        CollectionAssert.AreEqual(
            [ ("Alice", Score.Of 58)
              ("James", Score.Of 44)
              ("Jack", Score.Of 40)
              ("Bob", Score.Of 35)
              ("Lily", Score.Of 34)
              ("Michael", Score.Of 29) ],
            round4Finished.Standings()
        )
