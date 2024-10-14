using CopperDevs.DearImGui;
using CopperDevs.DearImGui.Renderer.Raylib;
using CopperDevs.DearImGui.Renderer.Raylib.Bindings;
using CopperDevs.DearImGui.Renderer.Raylib.Raylib_CSharp;
using CopperDevs.Logger;
using depression.ImGui;
using depression.Managers;
using Raylib_CSharp;
using Raylib_CSharp.Windowing;
using Riptide.Utils;
using Sparkle.CSharp;
using Sparkle.CSharp.Logging;
using SparkleLogger = Sparkle.CSharp.Logging.Logger;
using Sparkle.CSharp.Registries;
using Sparkle.CSharp.Scenes;

namespace depression;

public class Game : Sparkle.CSharp.Game
{
    public Game(GameSettings settings) : base(settings)
    {
        SparkleLogger.Message += Utility.CustomLog;
    }
    
    protected override void Init()
    {
        base.Init();

        CopperImGui.Setup<RlImGuiRenderer<RlImGuiBinding>>(true, false);
        CopperImGui.ShowDearImGuiAboutWindow = true;
        CopperImGui.ShowDearImGuiDemoWindow = false;
        CopperImGui.ShowDearImGuiMetricsWindow = false;
        CopperImGui.ShowDearImGuiDebugLogWindow = false; 
        CopperImGui.ShowDearImGuiIdStackToolWindow = false;

        Utility.SetWindowStyling();
        
        CopperImGui.ShowWindow<MainMenu>();
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
            s => Log.Network(s), 
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
        
        CopperImGui.Render();
        
        Window.SetTitle($"{Settings.Title} | Scene: {(SceneManager.ActiveScene != null ? SceneManager.ActiveScene.Name : "???")} [FPS: {Time.GetFPS()}]");
        
        NetworkManager.UpdateHandlers();
    }
    
    protected override void OnClose()
    {
        base.OnClose();

        CopperImGui.Shutdown();
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