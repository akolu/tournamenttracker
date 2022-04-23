module App.Context

open Elmish
open Feliz
open App.State

type ContextProps = State * (Msg -> unit)

let counterContext: Fable.React.IContext<ContextProps> = React.createContext ()
