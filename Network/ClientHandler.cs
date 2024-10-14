using CopperDevs.Logger;
using depression.Managers;
using Riptide;
using Riptide.Utils;

namespace depression.Network;

public static class ClientHandler
{
    public static void OnClientDisconnected(object? sender, DisconnectedEventArgs e)
    {
        if (e.Reason == DisconnectReason.Kicked)
        {
            RiptideLogger.Log(LogType.Info, "(CLIENT): Maybe server is closed, retrying connection...");

            Client? client = NetworkManager.StartClient();

            if (client == null) return;

            Thread checkingThread = new Thread(() =>
            {
                while (true)
                {
                    if (client is { IsConnecting: false, IsConnected: true })
                    {
                        Log.Network($"(CLIENT): Successfully reconnected to 127.0.0.1:{NetworkManager.CurrentPort}!");
                        return;
                    }

                    if (client is { IsConnecting: false, IsNotConnected: true })
                    {
                        NetworkManager.StopClient();
                        Log.Network("(CLIENT): Server is closed, Bye!!");
                        return;
                    }
                }
            });
            
            checkingThread.Start();
        }
    }
}