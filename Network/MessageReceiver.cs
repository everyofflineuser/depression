using System.Numerics;
using depression.Entities;
using depression.Extensions;
using depression.Managers;
using Riptide;
using Riptide.Utils;
using Sparkle.CSharp.Entities;
using Sparkle.CSharp.Scenes;
using LogType = Riptide.Utils.LogType;

namespace depression.Network;

public static class MessageReceiver
{
    [MessageHandler((ushort)MessageId.EntityUpdate)]
    private static void EntityUpdateServer(ushort fromClientId, Message message)
    {
        Vector3 position = message.GetVector3();
        Vector3 scale = message.GetVector3();
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
            .AddVector3(scale)
            .AddQuaternion(rotation)
            .AddUShort(entityId));
    }
    
    [MessageHandler((ushort)MessageId.EntityUpdate)]
    private static void EntityUpdateClient(Message message)
    {
        Vector3 position = message.GetVector3();
        Vector3 scale = message.GetVector3();
        Quaternion rotation = message.GetQuaternion();
        ushort entityId = message.GetUShort();

        Entity networkedEntity = SceneManager.ActiveScene!.GetEntity(entityId);
        networkedEntity.Position = position;
        networkedEntity.Rotation = rotation;
        networkedEntity.Scale = scale;

        Console.WriteLine($"Received NetworkEntity({entityId}) update" +
                          $"\nNew position: {position.X} {position.Y} {position.Z}" +
                          $"\nNew rotation: {rotation.X} {rotation.Y} {rotation.Z}" +
                          $"\nNew Scale: {scale.X} {scale.Y} {scale.Z}");
    }

    [MessageHandler((ushort)MessageId.Error)]
    private static void ErrorClient(Message message)
    {
        string error = message.GetString();
        
        RiptideLogger.Log(LogType.Error, error);
    }
}