namespace MqttGui.App

/// This is the main module of your application
/// here you handle all of your child pages as well as their
/// messages and their updates, useful to update multiple parts
/// of your application, Please refer to the `view` function
/// to see how to handle different kinds of "*child*" controls
module Shell =
    open Elmish
    open Avalonia
    open Avalonia.Controls
    open Avalonia.Input
    open Avalonia.FuncUI.DSL
    open Avalonia.FuncUI
    open Avalonia.FuncUI.Builder
    open Avalonia.FuncUI.Components.Hosts
    open Avalonia.FuncUI.Elmish

    type State =
      { AboutState: About.State 
        CounterState: Counter.State
        WatchState: Watch.State }

    type Msg =
        | AboutMsg of About.Msg
        | CounterMsg of Counter.Msg
        | WatchMsg of Watch.Msg

    let init=
        let aboutState, aboutCmd = About.init
        let counterState = Counter.init
        let watchState, watchCmd = Watch.init
        { AboutState = aboutState; CounterState = counterState; WatchState = watchState },
        Cmd.batch [ aboutCmd; Cmd.map WatchMsg watchCmd ]

    let update (msg: Msg) (state: State): State * Cmd<_> =
        match msg with
        | AboutMsg bpmsg ->
            let aboutState, cmd =
                About.update bpmsg state.AboutState
            { state with AboutState = aboutState },
            Cmd.map AboutMsg cmd
        | CounterMsg countermsg ->
            let counterState =
                Counter.update countermsg state.CounterState
            { state with CounterState = counterState },
            Cmd.none
        | WatchMsg watchMsg -> 
            let watchState, watchMsg' = Watch.update watchMsg state.WatchState
            { state with WatchState = watchState}, Cmd.map WatchMsg watchMsg'

    let view (state: State) (dispatch) =
        DockPanel.create
            [ DockPanel.children
                [ TabControl.create
                    [ TabControl.tabStripPlacement Dock.Top
                      TabControl.viewItems
                          [ TabItem.create
                                [ TabItem.header "Counter Sample"
                                  TabItem.content (Counter.view state.CounterState (CounterMsg >> dispatch)) ]
                            TabItem.create
                                [ TabItem.header "About"
                                  TabItem.content (About.view state.AboutState (AboutMsg >> dispatch)) ]
                            TabItem.create
                                [ TabItem.header "Watch"
                                  TabItem.content (Watch.view state.WatchState (WatchMsg >> dispatch)) ] ] ] ] ]

    type MainWindow() as this =
        inherit HostWindow()
        do
            base.Title <- "Full App"
            base.Width <- 800.0
            base.Height <- 600.0
            base.MinWidth <- 800.0
            base.MinHeight <- 600.0

            //this.VisualRoot.VisualRoot.Renderer.DrawFps <- true
            //this.VisualRoot.VisualRoot.Renderer.DrawDirtyRects <- true
            
            this.AttachDevTools()
            Elmish.Program.mkProgram (fun () -> init) update view
            |> Program.withHost this
            |> Program.run
