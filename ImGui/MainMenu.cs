using System.Numerics;
using CopperDevs.Core.Utility;
using CopperDevs.DearImGui;
using CopperDevs.DearImGui.Attributes;
using CopperDevs.DearImGui.Backend.Enums;
using CopperDevs.DearImGui.Rendering;
using depression.Managers;
using Sparkle.CSharp.Scenes;

namespace depression.ImGui;

[Window("Main Menu", WindowOpen = true, Flags = WindowFlags.NoCollapse | WindowFlags.NoScrollbar | WindowFlags.NoResize | WindowFlags.NoTitleBar | WindowFlags.NoMove)]
public class MainMenu : BaseWindow
{
    private MenuState _state = 0;
    private int _port = 7777;

    public override void WindowUpdate()
    {
        if (SceneManager.ActiveScene?.Name != "Main Menu")
        {
            CopperImGui.HideWindow<MainMenu>();
            return;
        }
        
        ImGuiNET.ImGui.SetWindowPos(Vector2.Zero.WithY(18));
        ImGuiNET.ImGui.SetWindowSize(ImGuiNET.ImGui.GetIO().DisplaySize.WithY(ImGuiNET.ImGui.GetIO().DisplaySize.Y - 18));

        switch (_state)
        {
            case MenuState.Normal:
                break;
            
            case MenuState.Multiplayer:
                Multiplayer();
                return;
        }
        
        CopperImGui.Button("Play Single-player", PlaySinglePlayer);
        CopperImGui.Button("Play Multi-player", () => _state = MenuState.Multiplayer);
    }

    private void PlaySinglePlayer()
    {
        SceneManager.SetScene(new Scenes.Test());
        CopperImGui.HideWindow<MainMenu>();
    }
    
    private void Multiplayer()
    {
        if (NetworkManager.CurrentPort <= 0) NetworkManager.CurrentPort = 7777;
        
        if (NetworkManager.CurrentPort != _port) NetworkManager.CurrentPort = (ushort)_port;
        
        ImGuiNET.ImGui.DragInt("Port", ref _port);
        
        CopperImGui.Button("Start Server", () => NetworkManager.StartServer(NetworkManager.CurrentPort,
            10));

        CopperImGui.Button("Start Client", () => NetworkManager.StartClient());
        
        CopperImGui.Button("Start Host", () =>
        {
            NetworkManager.StartServer(NetworkManager.CurrentPort,
                10);
            NetworkManager.StartClient();
        });
        
        CopperImGui.Button("Back to Main Menu", () => _state = MenuState.Normal);
    }
}

internal enum MenuState
{
    Normal = 0,
    Multiplayer = 1
}