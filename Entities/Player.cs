using System.Numerics;
using depression.Extensions;
using depression.Managers;
using Riptide;
using Riptide.Utils;
using Sparkle.CSharp.Entities.Components;
using Sparkle.CSharp.Scenes;

namespace depression.Entities;

public class Player : NetworkEntity
{
    private string name;
    private string username;
    internal static ushort ClientId;
    
    internal static Dictionary<ushort, Player> List = new();

    public Player(Vector3 position, string _name, ushort _Id) : base(position)
    {
        Tag = _name;
        name = _name;
        ClientId = _Id;
    }

    protected override void Dispose(bool disposing)
    {
        List.Remove(ClientId);
        base.Dispose(disposing);
    }

    internal static void Spawn(ushort id, string username, Vector3 position, bool shouldSendSpawn = false)
    {
        Player player;
        if (ClientId == NetworkManager.CurrentClient.Id)
        {
            RiptideLogger.Log(LogType.Info, "Spawning local player");
            
            player = new(position, $"Player {ClientId} ({username})", ClientId);
        }
        else
        {
            RiptideLogger.Log(LogType.Info, "Spawning player");
            
            player = new(position, $"Player {ClientId} ({username})", ClientId);
            player.AddComponent(new ModelRenderer(ContentRegistry.Models["Cube"], Vector3.Zero));
        }
        
        player.username = username;
        
        List.Add(ClientId, player);
        if (shouldSendSpawn)
            player.SendSpawn();
    }
    
    [MessageHandler((ushort)MessageId.SpawnPlayer)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector3());
    }
    
    private void SendSpawn()
    {
        Message message = Message.Create(MessageSendMode.Reliable, MessageId.SpawnPlayer);
        message.AddUShort(ClientId);
        message.AddString(Tag);
        message.AddVector3(Position);
        NetworkManager.CurrentClient.Send(message);
    }

    internal void SendSpawn(ushort newPlayerId)
    {
        Message message = Message.Create(MessageSendMode.Reliable, MessageId.SpawnPlayer);
        message.AddUShort(ClientId);
        message.AddString(username);
        message.AddVector3(Position);
        NetworkManager.CurrentServer.Send(message, newPlayerId);
    }
}