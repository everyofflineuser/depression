using CopperDevs.Logger;
using depression.Entities;
using depression.Network;
using depression.Scenes;
using Riptide;
using Riptide.Utils;
using Sparkle.CSharp.Entities;
using Sparkle.CSharp.Scenes;

namespace depression.Managers;

public static class NetworkManager
{
    public static Server? CurrentServer { get; private set; }
    public static Client? CurrentClient { get; private set; }

    public static string CurrentIP { get; set; } = "localhost";
    public static ushort CurrentPort { get; set; } = 7777;
    public static ushort MaxPlayers { get; set; } = 10;
    public static NetworkModes CurrentNetworkMode { get; set; } = NetworkModes.Multiplayer;
    
    private static readonly List<NetworkEntity> NetworkEntities = new ();
    
    public static Server? StartServer()
    {
        if (CurrentNetworkMode == NetworkModes.Singleplayer)
        {
            Log.Warning("Sorry, but you are in a singleplayer mode.");
            return null;
        }        
        
        Server server = new Server();
        server.Start(CurrentPort, MaxPlayers);
        
        CurrentServer = server;
        
        return server;
    }
    
    public static Client? StartClient()
    {
        if (CurrentNetworkMode == NetworkModes.Singleplayer)
        {
            Log.Warning("Sorry, but you are in a singleplayer mode.");
            return null;
        } 
        
        Client client = new Client();

        if (client.Connect($"{(CurrentIP == "localhost" ? "127.0.0.1" : CurrentIP)}:{CurrentPort}"))
        {
            CurrentClient = client;

            CurrentClient.Disconnected += ClientHandler.OnClientDisconnected;
            
            SceneManager.SetScene(new Test());

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

public enum NetworkModes
{
    Singleplayer,
    Multiplayer,
}