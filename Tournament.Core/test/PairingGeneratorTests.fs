module PairingGeneratorTests

open Tournament.PairingGenerator
open NUnit.Framework
open Tournament.Pairing

[<TestFixture>]
type TestClass() =

    let players =
        [ ("Alice", Score.Of 28)
          ("Bob", Score.Of 14)
          ("James", Score.Of 21)
          ("Michael", Score.Of 17) ]

    [<Test>]
    member this.``shuffle yields different result in two parallel shuffles``() =
        let playerLists = List.init 10 (fun _ -> shuffle players)
        let playerLists2 = List.init 10 (fun _ -> shuffle players)
        CollectionAssert.AreNotEqual(playerLists, playerLists2)

    [<Test>]
    member this.``shuffle retains all players in generated pairings``() =
        let shuffled = shuffle players
        Assert.AreEqual(2, shuffled.Length)

        let playersFromShuffledList =
            shuffled
            |> List.fold (fun acc (p1, p2) -> acc @ [ p1; p2 ]) []

        CollectionAssert.Contains(playersFromShuffledList, ("Alice", Score.Of 28))
        CollectionAssert.Contains(playersFromShuffledList, ("Bob", Score.Of 14))
        CollectionAssert.Contains(playersFromShuffledList, ("James", Score.Of 21))
        CollectionAssert.Contains(playersFromShuffledList, ("Michael", Score.Of 17))

    [<Test>]
    member this.``swiss orders player list by primary score with higher ranking player being player 1``() =
        let swissed = players |> swiss []

        Assert.AreEqual(
            [ (("Alice", Score.Of 28), ("James", Score.Of 21))
              (("Michael", Score.Of 17), ("Bob", Score.Of 14)) ],
            swissed
        )

    [<Test>]
    member this.``swiss orders players by secondary score if primary score is tied``() =
        let swissed =
            [ ("Bob", { Primary = 10; Secondary = 2 })
              ("Michael", { Primary = 10; Secondary = 1 })
              ("Alice", { Primary = 10; Secondary = 4 })
              ("James", { Primary = 10; Secondary = 3 }) ]
            |> swiss []

        CollectionAssert.AreEqual(
            [ ("Alice", { Primary = 10; Secondary = 4 }), ("James", { Primary = 10; Secondary = 3 })
              ("Bob", { Primary = 10; Secondary = 2 }), ("Michael", { Primary = 10; Secondary = 1 }) ],
            swissed
        )

    [<Test>]
    member this.``swiss orders players by alphabetically if primary and secondary scores are tied``() =
        let swissed =
            [ ("Bob", Score.Of 20)
              ("Michael", Score.Of 10)
              ("Alice", Score.Of 0)
              ("James", Score.Of 10) ]
            |> swiss []

        Assert.AreEqual(
            [ (("Bob", Score.Of 20), ("James", Score.Of 10))
              (("Michael", Score.Of 10), ("Alice", Score.Of 0)) ],
            swissed
        )

    [<Test>]
    member this.``swiss finds next unplayed opponent from the top if players would end up facing each other again``() =
        let history =
            [ ("James", "Alice")
              ("Bob", "Michael") ]

        let swissed = players |> swiss history

        Assert.AreEqual(
            [ (("Alice", Score.Of 28), ("Michael", Score.Of 17))
              (("James", Score.Of 21), ("Bob", Score.Of 14)) ],
            swissed
        )

    [<Test>]
    member this.``swiss takes into account situations where the last pairing ends up having already played each other``
        ()
        =
        let history =
            [ ("Alice", "Bob")
              ("James", "Michael")
              ("Lily", "Jack")
              ("Michael", "Alice")
              ("Jack", "Lily")
              ("Bob", "James") ]

        let players =
            [ ("Alice", Score.Of 30)
              ("Lily", Score.Of 30)
              ("Michael", Score.Of 21)
              ("James", Score.Of 20)
              ("Jack", Score.Of 10)
              ("Bob", Score.Of 9) ]

        let swissed = players |> swiss history

        Assert.AreEqual(
            [ (("Alice", Score.Of 30), ("Lily", Score.Of 30))
              (("Michael", Score.Of 21), ("Bob", Score.Of 9))
              (("James", Score.Of 20), ("Jack", Score.Of 10)) ],
            swissed
        )

    [<Test>]
    member this.``swiss can skip top pairing if it would cause the pairings to be impossible otherwise``() =
        // assume that 3 more players existed for 2 rounds (which Alice, Lily & Michael played) before dropping out
        // leaving history "incomplete" and creating a situation where the top pairing, Alice & Lily would be illegal
        // since it would make it impossible to pair the rest of the players
        let history =
            [ ("Jack", "Bob")
              ("James", "Jack")
              ("Bob", "James") ]

        let players =
            [ ("Alice", Score.Of 6)
              ("Lily", Score.Of 5)
              ("Jack", Score.Of 4)
              ("Michael", Score.Of 3)
              ("Bob", Score.Of 2)
              ("James", Score.Of 1) ]

        let swissed = players |> swiss history

        Assert.AreEqual(
            [ (("Alice", Score.Of 6), ("Jack", Score.Of 4))
              (("Lily", Score.Of 5), ("Bob", Score.Of 2))
              (("Michael", Score.Of 3), ("James", Score.Of 1)) ],
            swissed
        )
