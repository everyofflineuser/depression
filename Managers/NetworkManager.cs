using System.Numerics;
using Riptide;
using Riptide.Utils;
using Sparkle_Editor.Code.Entities;
using Sparkle.CSharp.Entities;
using Sparkle.CSharp.Scenes;

namespace Sparkle_Editor.Code.Managers;

public static class NetworkManager
{
    public static Server? CurrentServer { get; private set; }
    public static Client? CurrentClient { get; private set; }
    private static readonly List<NetworkEntity> NetworkEntities = new List<NetworkEntity>();
    
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

    public static void UpdateHandlers()
    {
        CurrentServer?.Update();
        CurrentClient?.Update();
    }

    public static bool IsActive() 
        => CurrentServer != null || CurrentClient != null;

    public static List<NetworkEntity> AddNetworkEntity(NetworkEntity entity)
    {
        NetworkEntities.Add(entity);
        
        return NetworkEntities;
    }
    
    public static List<NetworkEntity> RemoveNetworkEntity(NetworkEntity entity)
    {
        NetworkEntities.Remove(entity);
        
        return NetworkEntities;
    }

    public static List<NetworkEntity> GetNetworkEntities() => NetworkEntities;

    public static bool IsNetworkEntity(Entity entity)
    {
        return NetworkEntities.Contains(entity);
    }
}