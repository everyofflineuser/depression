using DiscordRPC;
using DiscordRPC.Logging;

namespace Sparkle_Editor.Code.Managers;

public static class DiscordManager
{
    public static DiscordRpcClient Client;

    /// <summary>
    /// Connect RPC and Subscribe to events
    /// </summary>
    public static void Initialize() 
    {
        Client = new DiscordRpcClient("1294695086088781844");          

        //Set the logger
        Client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

        //Connect to the RPC
        Client.Initialize();

        //Set the rich presence
        //Call this as many times as you want and anywhere in your code.
        Client.SetPresence(new RichPresence()
        {
            Details = "you in a depression, bro?",
            State = "Making game bruh",
            Assets = new Assets()
            {
                LargeImageKey = "logo",
                LargeImageText = "depression"
            },
            Timestamps = new Timestamps()
            {
                Start = DateTime.UtcNow
            }
        }); 
    }
}