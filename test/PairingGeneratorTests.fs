module PairingGeneratorTests

open System
open Tournament.PairingGenerator
open NUnit.Framework

[<TestFixture>]
type TestClass() =

    let unwrap res =
        match res with
        | Ok res -> res
        | Error err -> raise (Exception(err.ToString()))

    let players =
        [ ("Alice", 28)
          ("Bob", 14)
          ("James", 21)
          ("Michael", 17) ]

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

        CollectionAssert.Contains(playersFromShuffledList, ("Alice", 28))
        CollectionAssert.Contains(playersFromShuffledList, ("Bob", 14))
        CollectionAssert.Contains(playersFromShuffledList, ("James", 21))
        CollectionAssert.Contains(playersFromShuffledList, ("Michael", 17))

    [<Test>]
    member this.``swiss orders player list by score with higher ranking player being player 1``() =
        let swissed = players |> swiss []

        Assert.AreEqual(
            [ (("Alice", 28), ("James", 21))
              (("Michael", 17), ("Bob", 14)) ],
            swissed
        )

    [<Test>]
    member this.``swiss orders players by alphabetically if score is tied``() =
        let swissed =
            [ ("Bob", 20)
              ("Michael", 10)
              ("Alice", 0)
              ("James", 10) ]
            |> swiss []

        Assert.AreEqual(
            [ (("Bob", 20), ("James", 10))
              (("Michael", 10), ("Alice", 0)) ],
            swissed
        )

    [<Test>]
    member this.``swiss finds next unplayed opponent from the top if players would end up facing each other again``() =
        let history =
            [ ("James", "Alice")
              ("Bob", "Michael") ]

        let swissed = players |> swiss history

        Assert.AreEqual(
            [ (("Alice", 28), ("Michael", 17))
              (("James", 21), ("Bob", 14)) ],
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
            [ ("Alice", 30)
              ("Lily", 30)
              ("Michael", 21)
              ("James", 20)
              ("Jack", 10)
              ("Bob", 9) ]

        let swissed = players |> swiss history

        Assert.AreEqual(
            [ (("Alice", 30), ("Lily", 30))
              (("Michael", 21), ("Bob", 9))
              (("James", 20), ("Jack", 10)) ],
            swissed
        )

    [<Test>]
    [<Ignore("WIP")>]
    member this.``swiss can skip top pairing if it would cause the pairings to be impossible otherwise``() =
        // assume that 3 more players existed for 2 rounds (which Alice, Lily & Michael played) before dropping out
        // leaving history "incomplete" and creating a situation where the top pairing, Alice & Lily would be illegal
        // since it would make it impossible to pair the rest of the players
        let history =
            [ ("Jack", "Bob")
              ("James", "Jack")
              ("Bob", "James") ]

        let players =
            [ ("Alice", 6)
              ("Lily", 5)
              ("Jack", 4)
              ("Michael", 3)
              ("Bob", 2)
              ("James", 1) ]

        let swissed = players |> swiss history

        Assert.AreEqual(
            [ (("Alice", 6), ("Jack", 4))
              (("Lily", 5), ("Bob", 2))
              (("Michael", 3), ("James", 1)) ],
            swissed
        )
