using System.Numerics;
using CopperDevs.Logger;
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

        if (SceneManager.ActiveScene?.GetEntity(entityId) == null) return;
        
        if (!NetworkManager.IsNetworkEntity(SceneManager.ActiveScene.GetEntity(entityId)))
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
        MessageExtensions.SendAllEntitiesToClient(fromClientId);
        
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
    
    [MessageHandler((ushort)MessageId.SyncSpecifiedId)]
    private static void SyncRequestSpecifiedIdServer(ushort fromClientId, Message message)
    {
        MessageExtensions.SendEntityToClient(fromClientId, message.GetUShort());
    }
    
    [MessageHandler((ushort)MessageId.EntityUpdate)]
    private static void EntityUpdateClient(Message message)
    {
        // Проверяем, достаточно ли данных в сообщении
        if (message.UnreadBits < 4) // Например, если ожидается хотя бы 4 бита для ID
        {
            RiptideLogger.Log(LogType.Warning, "Недостаточно данных в сообщении для обновления сущности.");
            return;
        }

        Vector3 position = message.GetVector3();
        Vector3 scale = message.GetVector3();
        Quaternion rotation = message.GetQuaternion();
        ushort entityId = message.GetUShort();

        // Проверяем, существует ли сущность перед обновлением
        var entity = SceneManager.ActiveScene?.GetEntity(entityId);
        if (entity == null)
        {
            Log.Warning($"Сущность с ID {entityId} не найдена. Возможно, она еще не была создана.");
            
            Message syncRequestSpecifiedId = Message.Create(MessageSendMode.Reliable, MessageId.SyncSpecifiedId)
                .AddUShort(entityId);
            
            NetworkManager.CurrentClient!.Send(syncRequestSpecifiedId);
            
            return; // Выход из метода, если сущность не найдена
        }

        NetworkManager.UpdateEntity(entityId, position, scale, rotation);
    }
    
    [MessageHandler((ushort)MessageId.EntityCreate)]
    private static void EntityCreateClient(Message message)
    {
        string typeName = message.GetString(); // Получаем имя типа
        Vector3 position = message.GetVector3();
        Vector3 scale = message.GetVector3();
        Quaternion rotation = message.GetQuaternion();

        // Получаем тип из имени
        Type entityType = Type.GetType(typeName);
        if (entityType == null)
        {
            // Обработка ошибки, если тип не найден
            Log.Warning($"Тип {typeName} не найден.");
            return;
        }

        // Создаем экземпляр конкретного типа
        NetworkEntity entity = (NetworkEntity)Activator.CreateInstance(entityType);
        MessageExtensions.DeserializeEntityProperties(entity, message);
        
        SceneManager.ActiveScene!.AddEntity(entity);
        
        entity.Position = position;
        entity.Scale = scale;
        entity.Rotation = rotation;
        
        entity.InitSynced = true;
    }

    [MessageHandler((ushort)MessageId.Error)]
    private static void ErrorClient(Message message)
    {
        string error = message.GetString();
        
        RiptideLogger.Log(LogType.Error, error);
    }
}