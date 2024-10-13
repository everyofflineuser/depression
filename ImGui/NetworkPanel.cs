using CopperDevs.DearImGui;
using CopperDevs.DearImGui.Attributes;
using CopperDevs.DearImGui.Rendering;
using Sparkle_Editor.Code.Managers;

namespace Sparkle_Editor.Code.ImGui;

[Window("Network", WindowOpen = true)]
public class NetworkPanel : BaseWindow
{
    private int _port = NetworkManager.CurrentPort;
    private int _maxConnections = 10;
    
    public override void WindowUpdate()
    {
        base.WindowUpdate();

        if (NetworkManager.CurrentPort <= 0) NetworkManager.CurrentPort = 7777;
        if (_maxConnections <= 0) _maxConnections = 10;
        
        if (NetworkManager.CurrentPort != _port) NetworkManager.CurrentPort = (ushort)_port;
        
        ImGuiNET.ImGui.DragInt("Port", ref _port);
        ImGuiNET.ImGui.DragInt("Max Connections", ref _maxConnections);
        
        if (NetworkManager.CurrentServer == null)
        {
            CopperImGui.Button("Start Server", () => NetworkManager.StartServer((ushort)NetworkManager.CurrentPort,
                (ushort)_maxConnections));
        }
        else
        {
            CopperImGui.Button("Stop Server", NetworkManager.StopServer);
        }

        if (NetworkManager.CurrentClient == null)
        {
            CopperImGui.Button("Start Client", () => NetworkManager.StartClient());
        }
        else
        {
            CopperImGui.Button("Stop Client", NetworkManager.StopClient);
        }
        
        if (NetworkManager.CurrentServer == null) return;
        
        CopperImGui.Separator("Information about your server");

        CopperImGui.Text(
            $"Players on Server: {NetworkManager.CurrentServer?.ClientCount}/{NetworkManager.CurrentServer?.MaxClientCount}");
    }
}