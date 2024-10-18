using System.Numerics;
using depression.Entities;
using depression.Extensions;
using depression.Managers;
using Riptide;
using Riptide.Utils;
using Sparkle.CSharp.Scenes;

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
    
        NetworkManager.UpdateEntity(entityId, position, scale, rotation);
        
        // 0 is server
        NetworkManager.CurrentServer?.SendToAll(Message.Create(MessageSendMode.Reliable, 1)
            .AddVector3(position)
            .AddVector3(scale)
            .AddQuaternion(rotation)
            .AddUShort(entityId));
    }
    
    [MessageHandler((ushort)MessageId.Sync)]
    private static void SyncRequestServer(ushort fromClientId, Message message)
    {
        foreach (NetworkEntity entity in NetworkManager.GetNetworkEntities())
        {
            Message entityUpdate = Message.Create(MessageSendMode.Reliable, MessageId.EntityUpdate)
                .AddVector3(entity.Position)
                .AddVector3(entity.Scale)
                .AddQuaternion(entity.Rotation)
                .AddUShort((ushort)entity.Id);
            
            NetworkManager.CurrentServer!.Send(entityUpdate, fromClientId);
        }
    }
    
    [MessageHandler((ushort)MessageId.EntityUpdate)]
    private static void EntityUpdateClient(Message message)
    {
        Vector3 position = message.GetVector3();
        Vector3 scale = message.GetVector3();
        Quaternion rotation = message.GetQuaternion();
        ushort entityId = message.GetUShort();

        NetworkManager.UpdateEntity(entityId, position, scale, rotation);
    }

    [MessageHandler((ushort)MessageId.Error)]
    private static void ErrorClient(Message message)
    {
        string error = message.GetString();
        
        RiptideLogger.Log(LogType.Error, error);
    }
}