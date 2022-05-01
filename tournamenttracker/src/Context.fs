module Context

open Elmish
open Feliz
open State

type ContextProps = State * (Action -> unit)

let counterContext: Fable.React.IContext<ContextProps> = React.createContext ()

let tournamentContext: Fable.React.IContext<ContextProps> = React.createContext ()
