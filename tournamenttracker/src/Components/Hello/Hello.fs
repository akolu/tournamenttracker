module Components.HelloWorld

open Feliz

Fable.Core.JsInterop.importSideEffects "./Hello.css"

let HelloWorld () =
    Html.h1 [ prop.className "Hello"
              prop.text "Hello World" ]
