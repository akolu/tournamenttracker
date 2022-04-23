module App.Components.Hello

open Feliz

Fable.Core.JsInterop.importSideEffects "./Hello.scss"

[<ReactComponent>]
let Hello () =
    Html.h1 [ prop.className "Hello"
              prop.text "Hello World" ]
