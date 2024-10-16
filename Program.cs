using System.Reflection;
using depression.Managers;
using Raylib_CSharp.Windowing;
using Sparkle.CSharp;

namespace depression;

public static class Program
{
    public static Version Version = Assembly.GetEntryAssembly().GetName().Version;
    
    [STAThread]
    private static void Main(string[]? args)
    {
        ConfigFlags flags = ConfigFlags.VSyncHint | ConfigFlags.ResizableWindow;
        
        if (args != null && args.Contains("--server"))
        {
            flags = ConfigFlags.HiddenWindow;
            for (short i = 0; i < args.Length; i++)
            {
                if (args[i] == "--maxplayers")
                {
                    if (ushort.TryParse(args[++i], out ushort maxPlayers))
                    {
                        NetworkManager.MaxPlayers = maxPlayers;
                        break;
                    }
                }
            }
        }

        GameSettings settings = new GameSettings()
        {
            Title = "Depression",
            WindowFlags = flags
        };
        
        if (flags != ConfigFlags.HiddenWindow) DiscordManager.Initialize();
        
        Game game;
        if (flags == ConfigFlags.HiddenWindow) game = new Game(settings, true);
        else game = new Game(settings);
        game.Run(null);
    }
}