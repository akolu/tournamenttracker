module Components.IconButton

open Feliz
open Feliz.Bulma
open Fable.FontAwesome

Fable.Core.JsInterop.importSideEffects "./IconButton.scss"

type Components() =
    [<ReactComponent>]
    static member IconButton(icon: Fa.IconOption, props: IReactProperty list) =
        Bulma.button.button (
            [ button.isRounded
              button.isSmall
              prop.className "IconButton__button--root"
              prop.children [
                  Bulma.icon (Fa.i [ icon ] [])
              ] ]
            @ props
        )
