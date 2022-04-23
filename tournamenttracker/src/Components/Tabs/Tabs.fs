module App.Components.Tabs

open Feliz

Fable.Core.JsInterop.importSideEffects "./Tabs.scss"

type TabsProps =
    { onTabChanged: int -> unit
      items: ReactElement list }

[<ReactComponent>]
let Tabs =
    React.functionComponent (fun (props: TabsProps) ->
        let (selected, setSelected) = React.useState (0)

        let getTab (i: int) (x: ReactElement) : ReactElement =
            Html.li [
                prop.className (if (=) selected i then "active" else "")
                prop.children [ x ]
                prop.onClick (fun _ -> setSelected (i))
            ]

        React.useEffect ((fun _ -> props.onTabChanged (selected)), [| box selected |])

        Html.div [
            prop.className "Tabs__div__wrapper"
            prop.children [
                Html.ul [
                    prop.children (List.mapi getTab props.items)
                ]
                Html.div [
                    prop.className "Tabs__div__fill"
                    prop.children []
                ]
            ]
        ])
