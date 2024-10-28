using System.Numerics;
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
    
    private static readonly List<NetworkEntity> NetworkEntities = new ();
    
    public static Server? StartServer()
    {
        if (MaxPlayers == 0)
        {
            if (MaxPlayers > 0) MaxPlayers = 0;
            
            Log.Warning("Max players is set to zero!");
        }
        
        Server server = new Server();
        server.Start(CurrentPort, MaxPlayers);
        
        CurrentServer = server;
        
        return server;
    }
    
    public static Client? StartClient()
    {
        Client client = new Client();

        if (client.Connect($"{(CurrentIP == "localhost" ? "127.0.0.1" : CurrentIP)}:{CurrentPort}"))
        {
            CurrentClient = client;

            CurrentClient.Connected += ClientHandler.OnClientConnected;
            CurrentClient.Disconnected += ClientHandler.OnClientDisconnected;
            CurrentClient.ConnectionFailed += ClientHandler.OnConnectionFailed;
            
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
        if (CurrentClient == null &&
            CurrentServer == null) return;
        
        CurrentClient?.Send(msg);
    }

    public static Entity? UpdateEntity(ushort entityId, Vector3 position, Vector3 scale, Quaternion rotation)
    {
        if (SceneManager.ActiveScene == null ||
            SceneManager.ActiveScene.GetEntity(entityId) == null ||
            CurrentClient == null &&
            CurrentServer == null)
        {
            return null;
        }
        
        Entity tempEntity = SceneManager.ActiveScene!.GetEntity(entityId);
        tempEntity.Position = position;
        tempEntity.Rotation = rotation;
        tempEntity.Scale = scale;
        
        Log.Network($"Received NetworkEntity({entityId}) update" +
                          $"\nNew position: {position.X} {position.Y} {position.Z}" +
                          $"\nNew rotation: {rotation.X} {rotation.Y} {rotation.Z}" +
                          $"\nNew Scale: {scale.X} {scale.Y} {scale.Z}");
        
        return tempEntity;
    }
    
    public static void UpdateHandlers()
    {
        CurrentServer?.Update();
        CurrentClient?.Update();
    }

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