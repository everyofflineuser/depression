using System.Numerics;
using Riptide;
using Sparkle.CSharp.Entities;
using Sparkle.CSharp.Scenes;

namespace Sparkle_Editor.Code.Managers;

public static class NetworkManager
{
    public static Server? CurrentServer { get; private set; }
    public static Client? CurrentClient { get; private set; }
    
    public static Server StartServer(ushort port, ushort maxConnections)
    {
        Server server = new Server();
        server.Start(port, maxConnections);
        
        CurrentServer = server;

        return server;
    }

    public static void SendMessage(Message msg)
    {
        if (!IsActive()) return;
        
        CurrentClient?.Send(msg);
    }
    
    public static Client StartClient(ushort port)
    {
        Client client = new Client();
        client.Connect($"127.0.0.1:{port}");
        
        CurrentClient = client;

        return client;
    }
    
    [MessageHandler((ushort)MessageId.EntityUpdate)]
    private static void HandleServer(ushort fromClientId, Message message)
    {
        Vector3 position = message.GetVector3();
        Quaternion rotation = message.GetQuaternion();
        ushort entityId = message.GetUShort();
        
    
        // 0 is server
        CurrentServer?.SendToAll(Message.Create(MessageSendMode.Reliable, 1)
            .AddVector3(position)
            .AddQuaternion(rotation)
            .AddUShort(entityId));
    }
    
    [MessageHandler((ushort)MessageId.EntityUpdate)]
    private static void HandleClient(Message message)
    {
        Vector3 position = message.GetVector3();
        Quaternion rotation = message.GetQuaternion();
        ushort entityId = message.GetUShort();

        Entity networkedEntity = SceneManager.ActiveScene?.GetEntity(entityId)!;
        networkedEntity.Position = position;
        networkedEntity.Rotation = rotation;

        Console.WriteLine($"Received Entity({entityId}) update, New position: {position.X} {position.Y} {position.Z}, New rotation: {rotation.X} {rotation.Y} {rotation.Z}");
    }

    public static void UpdateHandlers()
    {
        CurrentServer?.Update();
        CurrentClient?.Update();
    }

    public static bool IsActive() 
        => CurrentServer != null || CurrentClient != null;
}