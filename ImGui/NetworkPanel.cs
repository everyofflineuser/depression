using System.Numerics;
using CopperDevs.DearImGui;
using CopperDevs.DearImGui.Attributes;
using CopperDevs.DearImGui.Rendering;
using ImGuiNET;
using Sparkle_Editor.Code.Managers;
using Sparkle.CSharp.Logging;
using Sparkle.CSharp.Scenes;

namespace Sparkle_Editor.Code.ImGui;

[Window("Network", WindowOpen = true)]
public class NetworkPanel : BaseWindow
{
    private int _port = 7777;
    private int _maxConnections = 10;
    private string _message = string.Empty;
    
    public override void WindowUpdate()
    {
        base.WindowUpdate();

        if (_port > 0) _port = 7777;
        if (_maxConnections > 0) _maxConnections = 10;
        
        ImGuiNET.ImGui.DragInt("Port", ref _port);
        ImGuiNET.ImGui.DragInt("Max Connections", ref _maxConnections);
        CopperImGui.Button("Start Server", () => NetworkManager.StartServer((ushort)_port,
            (ushort)_maxConnections));
        CopperImGui.Button("Start Client", () => NetworkManager.StartClient((ushort)_port));
        
        CopperImGui.Separator();

        ImGuiNET.ImGui.InputTextMultiline("Message", ref _message, 99, new Vector2(100,100));
        CopperImGui.Button("Send Message", () => NetworkManager.SendMessage(_message));
    }
}