using depression.Entities;
using depression.Network;
using Riptide;
using Riptide.Utils;
using Sparkle.CSharp.Entities;

namespace depression.Managers;

public static class NetworkManager
{
    public static Server? CurrentServer { get; private set; }
    public static Client? CurrentClient { get; private set; }

    public static ushort CurrentPort = 7777;
    
    private static readonly List<NetworkEntity> NetworkEntities = new ();
    
    public static Server StartServer(ushort port, ushort maxConnections)
    {
        Server server = new Server();
        server.Start(port, maxConnections);
        
        CurrentServer = server;

        return server;
    }
    
    public static Client? StartClient()
    {
        Client client = new Client();

        if (client.Connect($"127.0.0.1:{CurrentPort}"))
        {
            CurrentClient = client;

            CurrentClient.Disconnected += ClientHandler.OnClientDisconnected;

            return client;
        }
        else
        {
            RiptideLogger.Log(LogType.Error, "Failed to connect to the server.");
            return null;
        }
    }
    
    public static void StopServer()
    {
        foreach (Connection clientConnection in CurrentServer!.Clients)
        {
            CurrentServer.DisconnectClient(clientConnection);
        }
        
        CurrentServer.Stop();
        CurrentServer = null;
    }

    public static void StopClient()
    {
        CurrentClient!.Disconnect();
        CurrentClient = null;
    }

    public static void SendMessage(Message msg)
    {
        if (!IsActive()) return;
        
        CurrentClient?.Send(msg);
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