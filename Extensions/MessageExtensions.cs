using System.Numerics;
using CopperDevs.Logger;
using depression.Entities;
using depression.Managers;
using Riptide;

namespace depression.Extensions;

public enum MessageId : ushort
{
    EntityCreate = 0,
    EntityUpdate = 1,
    Sync = 2,
    SyncSpecifiedId = 3,
    Error = 999
}

public static class MessageExtensions
{
    #region Vector2
    /// <inheritdoc cref="AddVector2(Message, Vector2)"/>
    /// <remarks>This method is simply an alternative way of calling <see cref="AddVector2(Message, Vector2)"/>.</remarks>
    public static Message Add(this Message message, Vector2 value) => AddVector2(message, value);

    /// <summary>Adds a <see cref="Vector2"/> to the message.</summary>
    /// <param name="value">The <see cref="Vector2"/> to add.</param>
    /// <returns>The message that the <see cref="Vector2"/> was added to.</returns>
    public static Message AddVector2(this Message message, Vector2 value)
    {
        return message.AddFloat(value.X).AddFloat(value.Y);
    }

    /// <summary>Retrieves a <see cref="Vector2"/> from the message.</summary>
    /// <returns>The <see cref="Vector2"/> that was retrieved.</returns>
    public static Vector2 GetVector2(this Message message)
    {
        return new Vector2(message.GetFloat(), message.GetFloat());
    }
    #endregion

    #region Vector3
    /// <inheritdoc cref="AddVector3(Message, Vector3)"/>
    /// <remarks>This method is simply an alternative way of calling <see cref="AddVector3(Message, Vector3)"/>.</remarks>
    public static Message Add(this Message message, Vector3 value) => AddVector3(message, value);

    /// <summary>Adds a <see cref="Vector3"/> to the message.</summary>
    /// <param name="value">The <see cref="Vector3"/> to add.</param>
    /// <returns>The message that the <see cref="Vector3"/> was added to.</returns>
    public static Message AddVector3(this Message message, Vector3 value)
    {
        return message.AddFloat(value.X).AddFloat(value.Y).AddFloat(value.Z);
    }

    /// <summary>Retrieves a <see cref="Vector3"/> from the message.</summary>
    /// <returns>The <see cref="Vector3"/> that was retrieved.</returns>
    public static Vector3 GetVector3(this Message message)
    {
        return new Vector3(message.GetFloat(), message.GetFloat(), message.GetFloat());
    }
    #endregion

    #region Quaternion
    /// <inheritdoc cref="AddQuaternion(Message, Quaternion)"/>
    /// <remarks>This method is simply an alternative way of calling <see cref="AddQuaternion(Message, Quaternion)"/>.</remarks>
    public static Message Add(this Message message, Quaternion value) => AddQuaternion(message, value);

    /// <summary>Adds a <see cref="Quaternion"/> to the message.</summary>
    /// <param name="value">The <see cref="Quaternion"/> to add.</param>
    /// <returns>The message that the <see cref="Quaternion"/> was added to.</returns>
    public static Message AddQuaternion(this Message message, Quaternion value)
    {
        return message.AddFloat(value.X).AddFloat(value.Y).AddFloat(value.Z).AddFloat(value.W);
    }

    /// <summary>Retrieves a <see cref="Quaternion"/> from the message.</summary>
    /// <returns>The <see cref="Quaternion"/> that was retrieved.</returns>
    public static Quaternion GetQuaternion(this Message message)
    {
        return new Quaternion(message.GetFloat(), message.GetFloat(), message.GetFloat(), message.GetFloat());
    }
    #endregion

    public static void SerializeEntityProperties(NetworkEntity entity, Message message)
    {
        var properties = entity.GetType().GetProperties();

        foreach (var property in properties)
        {
            if (property.CanRead && property.CanWrite)
            {
                try
                {
                    var value = property.GetValue(entity);

                    // Сериализуем свойства в зависимости от их типа
                    switch (value)
                    {
                        case string stringValue:
                            message.AddString(stringValue);
                            break;

                        case int intValue:
                            message.AddInt(intValue);
                            break;

                        case float floatValue:
                            message.AddFloat(floatValue);
                            break;

                        case Vector3 vectorValue:
                            message.AddVector3(vectorValue);
                            break;

                        case Quaternion quaternionValue:
                            message.AddQuaternion(quaternionValue);
                            break;

                        // Добавьте дополнительные типы, если это необходимо
                        // Например, для массивов или коллекций
                        case Array arrayValue:
                            message.AddInt(arrayValue.Length); // Сначала добавляем длину массива
                            foreach (var item in arrayValue)
                            {
                                // Обработка элементов массива в зависимости от их типа
                                switch (item)
                                {
                                    case int itemValue:
                                        message.AddInt(itemValue);
                                        break;
                                    // Добавьте другие типы по мере необходимости
                                }
                            }

                            break;
                        
                        case uint uintValue:
                            message.AddUInt(uintValue); // Предполагается, что у вас есть метод AddUInt
                            break;
                        
                        case ushort ushortValue:
                            message.AddUShort(ushortValue); // Предполагается, что у вас есть метод AddUInt
                            break;

                        default:
                            // Обработка случая, когда тип не поддерживается
                            Log.Warning(
                                $"Тип {property.PropertyType} не поддерживается для сериализации.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Ошибка при сериализации свойства {property.Name}: {ex.Message}");
                }
            }
        }
    }

    public static void DeserializeEntityProperties(NetworkEntity entity, Message message)
    {
        var properties = entity.GetType().GetProperties();

        foreach (var property in properties)
        {
            if (property.Name.Equals("Id")) continue;
            
            if (property.CanRead && property.CanWrite)
            {
                try
                {
                    // Десериализуем свойства в зависимости от их типа
                    switch (property.PropertyType)
                    {
                        case Type stringType when stringType == typeof(string):
                            property.SetValue(entity, message.GetString());
                            break;

                        case Type intType when intType == typeof(int):
                            property.SetValue(entity, message.GetInt());
                            break;

                        case Type floatType when floatType == typeof(float):
                            property.SetValue(entity, message.GetFloat());
                            break;

                        case Type vector3Type when vector3Type == typeof(Vector3):
                            property.SetValue(entity, message.GetVector3());
                            break;

                        case Type quaternionType when quaternionType == typeof(Quaternion):
                            property.SetValue(entity, message.GetQuaternion());
                            break;

                        // Добавьте дополнительные типы, если это необходимо
                        // Например, для массивов или коллекций
                        case Type arrayType when arrayType.IsArray:
                            var arrayLength = message.GetInt();
                            var array = Array.CreateInstance(property.PropertyType.GetElementType(), arrayLength);
                            for (int i = 0; i < arrayLength; i++)
                            {
                                // Десериализуем элементы массива в зависимости от их типа
                                switch (property.PropertyType.GetElementType())
                                {
                                    case Type elementType when elementType == typeof(int):
                                        array.SetValue(message.GetInt(), i);
                                        break;
                                    // Добавьте другие типы по мере необходимости
                                }
                            }

                            property.SetValue(entity, array);
                            break;
                        
                        case Type uIntType when uIntType == typeof(uint):
                            property.SetValue(entity, message.GetUInt());
                            break;
                        
                        case Type uShortType when uShortType == typeof(ushort):
                            property.SetValue(entity, message.GetUShort());
                            break;

                        default:
                            // Обработка случая, когда тип не поддерживается
                            Log.Warning(
                                $"Тип {property.PropertyType} не поддерживается для десериализации.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(
                        $"Ошибка при десериализации свойства {property.Name}: {ex.Message}");
                }
            }
        }
    }
    
    public static void SendAllEntitiesToClient(ushort clientId)
    {
        foreach (NetworkEntity entity in NetworkManager.GetNetworkEntities())
        {
            Message entityCreate = Message.Create(MessageSendMode.Reliable, MessageId.EntityCreate)
                .AddString(entity.GetType().AssemblyQualifiedName)
                .AddVector3(entity.Position)
                .AddVector3(entity.Scale)
                .AddQuaternion(entity.Rotation); // Добавляем ID сущности
            
            SerializeEntityProperties(entity, entityCreate);
            NetworkManager.CurrentServer!.Send(entityCreate, clientId);
        }
    }

    public static void SendEntityToClient(ushort fromClientId, ushort entityId)
    {
        // Получаем сущность по ID
        NetworkEntity entity = GetEntityById(entityId);
    
        // Проверяем, существует ли сущность
        if (entity == null)
        {
            Log.Warning($"Сущность с ID {entityId} не найдена. Не удается отправить клиенту.");
            return; // Выход из метода, если сущность не найдена
        }

        // Продолжайте с отправкой данных сущности клиенту
        Message entityUpdate = Message.Create(MessageSendMode.Reliable, MessageId.EntityUpdate)
            .AddVector3(entity.Position)
            .AddVector3(entity.Scale)
            .AddQuaternion(entity.Rotation)
            .AddUShort(entityId);
    
        NetworkManager.CurrentServer!.Send(entityUpdate, fromClientId);
    }
    
    private static NetworkEntity GetEntityById(ushort entityId)
    {
        // Предположим, у вас есть список всех сущностей
        return NetworkManager.GetNetworkEntities().FirstOrDefault(e => e.Id == entityId);
    }
}