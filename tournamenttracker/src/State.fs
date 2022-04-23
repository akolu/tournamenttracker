module App.State

open Elmish

type State = { Count: int }

let state () = { Count = 0 }, Cmd.none

type Msg =
    | Increment
    | Decrement

let update (msg: Msg) (state: State) =
    match msg with
    | Increment -> { state with Count = state.Count + 1 }, Cmd.none
    | Decrement -> { state with Count = state.Count - 1 }, Cmd.none
