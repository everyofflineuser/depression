using System.Reflection;
using depression.Managers;
using Raylib_CSharp.Windowing;
using Sparkle.CSharp;

namespace depression;

public static class Program
{
    public static Version Version = Assembly.GetEntryAssembly()?.GetName().Version!;
    
    [STAThread]
    private static void Main(string[]? args)
    {
        GameSettings settings = new GameSettings()
        {
            Title = "Depression"
        };
        
        DiscordManager.Initialize();
        
        Game game = new Game(settings);
        game.Run(null);
    }
}