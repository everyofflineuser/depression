using System.Numerics;
using depression.Entities;
using depression.Extensions;
using depression.Managers;
using Riptide;
using Riptide.Utils;
using Sparkle.CSharp.Scenes;
using LogType = Riptide.Utils.LogType;

namespace depression.Network;

public static class MessageReceiver
{
    [MessageHandler((ushort)MessageId.EntityUpdate)]
    private static void EntityUpdateServer(ushort fromClientId, Message message)
    {
        Vector3 position = message.GetVector3();
        Quaternion rotation = message.GetQuaternion();
        ushort entityId = message.GetUShort();

        if (!NetworkManager.IsNetworkEntity(SceneManager.ActiveScene?.GetEntity(entityId)!))
        {
            Message error = Message.Create(MessageSendMode.Reliable, MessageId.Error)
                .AddString("Bruh, is it a network entity?");

            NetworkManager.CurrentServer?.Send(error, fromClientId);
            return;
        }
    
        // 0 is server
        NetworkManager.CurrentServer?.SendToAll(Message.Create(MessageSendMode.Reliable, 1)
            .AddVector3(position)
            .AddQuaternion(rotation)
            .AddUShort(entityId));
    }
    
    [MessageHandler((ushort)MessageId.EntityUpdate)]
    private static void EntityUpdateClient(Message message)
    {
        Vector3 position = message.GetVector3();
        Quaternion rotation = message.GetQuaternion();
        ushort entityId = message.GetUShort();

        NetworkEntity networkedEntity = (SceneManager.ActiveScene?.GetEntity(entityId)! as NetworkEntity)!;
        networkedEntity.Position = position;
        networkedEntity.Rotation = rotation;

        Console.WriteLine($"Received NetworkEntity({entityId}) update, New position: {position.X} {position.Y} {position.Z}, New rotation: {rotation.X} {rotation.Y} {rotation.Z}");
    }

    [MessageHandler((ushort)MessageId.Error)]
    private static void ErrorClient(Message message)
    {
        string error = message.GetString();
        
        RiptideLogger.Log(LogType.Error, error);
    }
}