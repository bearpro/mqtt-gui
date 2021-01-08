namespace MqttGui.Broker

open System

open MqttGui.Broker.Types

type MessageReceivedHandler = delegate of Message -> unit

type IMessageInterceptor =
    
    [<CLIEvent>]
    abstract member OnMessage : IEvent<Message>