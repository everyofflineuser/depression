using System.Reflection;
using Sparkle_Editor.Code.Managers;
using Sparkle.CSharp;

namespace Sparkle_Editor.Code;

public static class Program
{
    public static Version Version = Assembly.GetEntryAssembly().GetName().Version;
    
    [STAThread]
    private static void Main(string[] args)
    {
        GameSettings settings = new GameSettings()
        {
            Title = "Depression"
        };
        
        DiscordManager.Initialize();
        
        using Game game = new Game(settings);
        game.Run(new Scenes.MainMenu());
    }
}