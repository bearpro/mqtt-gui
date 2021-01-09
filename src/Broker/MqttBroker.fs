namespace MqttGui.Broker

open System

open MQTTnet
open MQTTnet.Server

open MqttGui.Broker.Types

type MqttBroker() =
    let server = MqttFactory().CreateMqttServer()

    let messageIntercepted = new Event<Message> ()

    let receiveMessage (context: MqttApplicationMessageInterceptorContext) =
        let message = context.ApplicationMessage
        let message' = { Topic   = message.Topic
                         Payload = message.Payload }
        messageIntercepted.Trigger message'

    let options = 
        MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(1883)
            .WithApplicationMessageInterceptor(receiveMessage)
            .Build()

    let runAsync () = async {
        let task = server.StartAsync options
        do! Async.AwaitTask task
    }


    do runAsync() |> Async.Start

    interface IMessageInterceptor with
        [<CLIEvent>]
        member _.OnMessage = messageIntercepted.Publish