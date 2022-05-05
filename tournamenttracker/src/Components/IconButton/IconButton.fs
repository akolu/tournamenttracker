module Components.IconButton

open Feliz
open Feliz.Bulma
open Fable.FontAwesome
open Elmish

[<ReactComponent>]
let IconButton (icon: Fa.IconOption, fn: Browser.Types.MouseEvent -> unit) =
    Bulma.button.button [
        button.isRounded
        button.isSmall
        prop.onClick fn
        prop.className "IconButton__button--root"
        prop.children [
            Bulma.icon (Fa.i [ icon ] [])
        ]
    ]
