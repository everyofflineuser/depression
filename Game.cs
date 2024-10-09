using System.Diagnostics;
using Raylib_CSharp;
using Raylib_CSharp.Windowing;
using Riptide;
using Riptide.Utils;
using Sparkle_Editor.Code.Managers;
using Sparkle_Editor.Code.ImGui;
using Sparkle.CSharp;
using Sparkle.CSharp.Logging;
using Sparkle.CSharp.Overlays;
using Sparkle.CSharp.Registries;

namespace Sparkle_Editor.Code;

public class Game : Sparkle.CSharp.Game
{
    private string _title;
    public Server Server;

    public Game(GameSettings settings, string title = "Sparkle Game") : base(settings)
    {
        _title = title;
    }

    protected override void Init() 
    {
        base.Init();
        
        Window.SetTitle(_title);
        
        var myOverlay = new DearImGuiOverlay("DearImGui Overlay")
        {
            Enabled = true,
        };
        OverlayManager.Add(myOverlay);
    }
    
    protected override void OnRun()
    {
        base.OnRun();
        RegistryManager.Add(new ContentRegistry());

        if (Program.Version.Major <= 1)
        {
            Logger.Warn("You are using a alpha branch! v" + Program.Version);
        }
        
        RiptideLogger.Initialize(s => Logger.Debug(s), s => Logger.Info(s), s => Logger.Warn(s), s => Logger.Error(s), false);
    }

    protected override void Draw() 
    {
        base.Draw();
        
        Window.SetTitle($"{_title} [FPS: {Time.GetFPS()}]");
        
        NetworkManager.UpdateHandlers();
    }


    protected override void OnClose()
    {
        base.OnClose();

        DiscordManager.Client.Dispose();
        NetworkManager.CurrentServer?.Stop();
        NetworkManager.CurrentClient?.Disconnect();
        Environment.Exit(0);
    }
}