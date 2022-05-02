module Components.TournamentContainer

open Feliz
open Fable.FontAwesome
open Fable.FontAwesome.Free
open Components.Tabs
open Components.Settings
open Components.Round
open Components.Results
open Feliz.Bulma
open State
open Context

Fable.Core.JsInterop.importSideEffects "./TournamentContainer.scss"

[<ReactComponent>]
let TournamentContainer () =

    let (state, dispatch) = React.useContext (tournamentContext)
    let (tab, setTab) = React.useState (0)

    React.useEffect (
        (fun _ ->
            match state.Tournament.CurrentRound with
            | Some r when tab <> r.Number -> setTab (r.Number)
            | None when state.Tournament.Rounds.Length > 0 -> setTab (state.Tournament.Rounds.Length + 1)
            | _ -> ()),
        [| unbox state.Tournament.CurrentRound |]
    )

    let onTournamentCreated settings = dispatch (CreateTournament settings)

    let getActiveTab =
        match tab with
        | 0 -> Settings(onTournamentCreated)
        | num when num > state.Tournament.Rounds.Length -> Results()
        | num ->
            Round(
                state.Tournament.Rounds
                |> List.find (fun r -> ((=) r.Number num))
            )

    Html.div [
        prop.className "TournamentContainer__div--root"
        prop.children [
            Tabs
                {| onTabChanged = setTab
                   selected = tab
                   rounds = state.Tournament.Rounds |}
            Html.div [
                prop.className "TournamentContainer__div--content"
                prop.children getActiveTab
            ]
        ]
    ]
