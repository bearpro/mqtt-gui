namespace MqttGui.App

open System

open Elmish
open Avalonia.Controls.Templates
open Avalonia.Controls
open Avalonia.FuncUI.DSL

open MqttGui.Broker
open MqttGui.Broker.Types

module Watch =
    type TopicTree = { Name: string
                       Messages: Message list
                       SubTopics: TopicTree list }
    type State = 
      { Topics: TopicTree
        MessageInterceptor: IMessageInterceptor }

    type Msg = 
    | NewMessage of Message

    let init = 
        let state = { Topics = { Name = "/"; Messages = []; SubTopics = 
                      [ { Name = "Topic1"; Messages = []; SubTopics = [] } 
                        { Name = "Topic2"; Messages = []; SubTopics = [] } ] }
                      MessageInterceptor = new MqttBroker() }
        let cmd = 
            Cmd.OfAsync.perform Async.AwaitEvent state.MessageInterceptor.OnMessage NewMessage
        state, cmd

    let update msg state = 
        state, Cmd.none

    let view state dispatch =
        DockPanel.create [
            DockPanel.children [
                TreeView.create [
                    TreeView.width 230.0
                    TreeView.dock Dock.Left
                    TreeView.dataItems (Seq.singleton state.Topics)
                    TreeView.itemTemplate (
                        FuncTreeDataTemplate<TopicTree>(
                            (fun topic _ -> TextBlock(Text = topic.Name) :> Control),
                            (fun x -> x.SubTopics :> Collections.IEnumerable)
                        )
                    )
                ]
                Grid.create []
            ]
        ]
        