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
using Sparkle.CSharp.Scenes;

namespace Sparkle_Editor.Code;

public class Game : Sparkle.CSharp.Game
{
    public Game(GameSettings settings) : base(settings)
    {
    }

    protected override void Init() 
    {
        base.Init();
        
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
        
        RiptideLogger.Initialize(s => Logger.Debug(s), 
            s => Logger.Info(s), 
            s => Logger.Warn(s), 
            s => Logger.Error(s), false);
        
        //Subscribe to events
        DiscordManager.Client.OnReady += (sender, e) =>
        {
            Logger.Info($"Received Ready from user {e.User.Username}");
        };

        DiscordManager.Client.OnPresenceUpdate += (sender, e) =>
        {
            Logger.Info($"Received Update! {e.Presence}");
        };
    }

    protected override void Draw() 
    {
        base.Draw();
        
        Window.SetTitle($"{Settings.Title} | Scene: {(SceneManager.ActiveScene != null ? SceneManager.ActiveScene.Name : "???")} [FPS: {Time.GetFPS()}]");
        
        NetworkManager.UpdateHandlers();
    }


    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        
        DiscordManager.Client.Dispose();
        NetworkManager.CurrentServer?.Stop();
        NetworkManager.CurrentClient?.Disconnect();
        Environment.Exit(0);
    }
}