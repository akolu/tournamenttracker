module App.Components.Round

open Feliz

Fable.Core.JsInterop.importSideEffects "./Round.scss"

[<ReactComponent>]
let Round () = Html.div [ prop.className "Round" ]
