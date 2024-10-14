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
        
        CopperImGui.Space(5 * (ImGuiNET.ImGui.GetIO().DisplaySize.Y / 15));
        
        CopperImGui.Button("Play Single-player", ImGuiNET.ImGui.GetIO().DisplaySize.X - 18, 48, PlaySinglePlayer);
        CopperImGui.Button("Play Multi-player", ImGuiNET.ImGui.GetIO().DisplaySize.X - 18, 48, () => _state = MenuState.Multiplayer);
    }

    private void PlaySinglePlayer()
    {
        NetworkManager.CurrentNetworkMode = NetworkModes.Singleplayer;
        
        SceneManager.SetScene(new Scenes.Test());
        CopperImGui.HideWindow<MainMenu>();
    }
    
    private void Multiplayer()
    {
        if (_port <= 0) _port = 7777;
        
        if (NetworkManager.CurrentPort != _port) NetworkManager.CurrentPort = (ushort)_port;
        
        CopperImGui.Button("<= Back", ImGuiNET.ImGui.GetIO().DisplaySize.X / 10, 24, () => _state = MenuState.Normal);

        NetworkManager.CurrentNetworkMode = NetworkModes.Multiplayer;
        
        CopperImGui.Space(5 * (ImGuiNET.ImGui.GetIO().DisplaySize.Y / 15));
        
        CopperImGui.DragValue("Port", ref _port);
        
        CopperImGui.Button("Start Server", ImGuiNET.ImGui.GetIO().DisplaySize.X / 1.5f, 48, () => NetworkManager.StartServer(NetworkManager.CurrentPort,
            10));

        CopperImGui.Button("Start Client", ImGuiNET.ImGui.GetIO().DisplaySize.X / 1.5f, 48, () => NetworkManager.StartClient());
        
        CopperImGui.Button("Start Host", ImGuiNET.ImGui.GetIO().DisplaySize.X / 1.5f, 48, () =>
        {
            NetworkManager.StartServer(NetworkManager.CurrentPort,
                10);
            NetworkManager.StartClient();
        });
    }
}

internal enum MenuState
{
    Normal = 0,
    Multiplayer = 1
}