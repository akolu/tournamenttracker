module App.Components.TournamentContainer

open Feliz
open App.Components.Hello
open App.Components.Counter

Fable.Core.JsInterop.importSideEffects "./TournamentContainer.scss"

[<ReactComponent>]
let TournamentContainer () =
    Html.div [ prop.className "TournamentContainer__div__root"
               prop.children [ Hello(); Counter() ] ]
