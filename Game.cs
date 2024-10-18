using CopperDevs.DearImGui;
using CopperDevs.DearImGui.Renderer.Raylib;
using CopperDevs.DearImGui.Renderer.Raylib.Raylib_CSharp;
using CopperDevs.Logger;
using depression.ImGui;
using depression.Managers;
using depression.Scenes;
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
    private bool _isServer;
    
    public Game(GameSettings settings, bool isServer = false) : base(settings)
    {
        SparkleLogger.Message += Utility.CustomLog;
        this._isServer = isServer;
    }
    
    protected override void Init()
    {
        base.Init();
        
        CopperImGui.Setup<RlImGuiRenderer<RlImGuiBinding>>(true, false);
        CopperImGui.ShowDearImGuiAboutWindow = false;
        CopperImGui.ShowDearImGuiDemoWindow = false;
        CopperImGui.ShowDearImGuiMetricsWindow = false;
        CopperImGui.ShowDearImGuiDebugLogWindow = false; 
        CopperImGui.ShowDearImGuiIdStackToolWindow = false;

        Utility.SetWindowStyling();
        
        if (_isServer)
        {
            SceneManager.SetScene(new Test(true));
            NetworkManager.StartServer();
        }
        else
        {
            SceneManager.SetScene(new Empty());
        }
    }

    protected override void OnRun()
    {
        base.OnRun();
        RegistryManager.Add(new ContentRegistry());

        if (Program.Version.Major <= 1)
        {
            Log.Warning("You are using a alpha branch! v" + Program.Version);
        }
        
        if (_isServer) Log.UserAction("You are running the game in server mode!");
        
        RiptideLogger.Initialize(s => Log.Debug(s), 
            s => Log.Network(s), 
            s => Log.Warning(s), 
            s => Log.Error(s), false);
        
        
        if (_isServer) return;
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
        
        NetworkManager.UpdateHandlers();
        
        if (_isServer) return;
        CopperImGui.Render();
        
        Window.SetTitle($"{Settings.Title} | Scene: {(SceneManager.ActiveScene != null ? SceneManager.ActiveScene.Name : "???")} [FPS: {Time.GetFPS()}]");
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (NetworkManager.CurrentClient != null ||
            NetworkManager.CurrentServer != null)
        {
            if (SceneManager.ActiveScene?.Name == "Test")
            {
                CopperImGui.ShowWindow<NetworkPanel>();
                CopperImGui.HideWindow<MainMenu>();
            }
            else
            {
                SceneManager.SetScene(new Test());
            }
        }
        else
        {
            if (SceneManager.ActiveScene?.Name == "Empty")
            {
                CopperImGui.ShowWindow<MainMenu>();
                CopperImGui.HideWindow<NetworkPanel>();
            }
            else
            {
                SceneManager.SetScene(new Empty());
            }
        }
    }

    protected override void OnClose()
    {
        base.OnClose();

        if (_isServer) return;
        
        CopperImGui.Shutdown();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        
        NetworkManager.CurrentServer?.Stop();
        NetworkManager.CurrentClient?.Disconnect();
        if (!_isServer) DiscordManager.Client.Dispose();
        Environment.Exit(0);
    }
}