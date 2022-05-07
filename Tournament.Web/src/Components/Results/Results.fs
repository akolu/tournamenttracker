module Components.Results

open Feliz

Fable.Core.JsInterop.importSideEffects "./Results.scss"

[<ReactComponent>]
let Results () = Html.div "Results"
