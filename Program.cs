using System.Reflection;
using depression.Managers;
using Sparkle.CSharp;

namespace depression;

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