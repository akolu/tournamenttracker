module Context

open Elmish
open Feliz
open State

type ContextProps = State * (Action -> unit)

let tournamentContext: Fable.React.IContext<ContextProps> = React.createContext ()
