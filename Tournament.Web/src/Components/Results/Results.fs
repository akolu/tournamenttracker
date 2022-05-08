module Components.Results

open Feliz
open Tournament.Tournament
open Components.Standings

Fable.Core.JsInterop.importSideEffects "./Results.scss"

[<ReactComponent>]
let Results (tournament: Tournament) =
    Html.div [
        prop.className "Results__root"
        prop.children [
            Bulma.Divider.divider "Results"
            Standings(tournament.Rounds, tournament.Standings())
        ]
    ]
