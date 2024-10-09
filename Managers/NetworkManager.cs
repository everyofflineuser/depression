using Riptide;

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

    public static Message SendMessage(string msg)
    {
        Message message = Message.Create(MessageSendMode.Reliable, 1);
        message.AddString(msg);

        CurrentServer?.SendToAll(message);
        CurrentClient?.Send(message);

        return message;
    }
    
    public static Client StartClient(ushort port)
    {
        Client client = new Client();
        client.Connect($"127.0.0.1:{port}");
        
        CurrentClient = client;

        return client;
    }
    
    [MessageHandler(1)]
    private static void HandleServer(ushort fromClientId, Message message)
    {
        string someString = message.GetString();
    
        // 0 is server
        CurrentServer?.SendToAll(Message.Create(MessageSendMode.Reliable, 1).AddString(someString).AddUShort(fromClientId));
    }
    
    [MessageHandler(1)]
    private static void HandleClient(Message message)
    {
        string someString = message.GetString();
        ushort fromClientId = message.GetUShort();
        
        Console.WriteLine($"{fromClientId}: {someString}");
    }

    public static void UpdateHandlers()
    {
        CurrentServer?.Update();
        CurrentClient?.Update();
    }

    public static bool IsActive() 
        => CurrentServer != null || CurrentClient != null;
}