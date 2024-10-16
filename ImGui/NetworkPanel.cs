using CopperDevs.DearImGui;
using CopperDevs.DearImGui.Attributes;
using CopperDevs.DearImGui.Rendering;
using depression.Managers;
using Sparkle.CSharp.Scenes;

namespace depression.ImGui;

[Window("Network", WindowOpen = false)]
public class NetworkPanel : BaseWindow
{
    private string _ip = NetworkManager.CurrentIP;
    private int _port = NetworkManager.CurrentPort;
    private int _maxConnections = 10;
    
    public override void WindowUpdate()
    {
        if (SceneManager.ActiveScene?.Name != "Test" /*|| NetworkManager.CurrentNetworkMode == NetworkModes.Singleplayer*/)
        {
            CopperImGui.HideWindow<NetworkPanel>();
            return;
        }
        
        if (NetworkManager.CurrentPort <= 0) NetworkManager.CurrentPort = 7777;
        if (_maxConnections <= 0) _maxConnections = 10;
        
        if (NetworkManager.CurrentPort != _port) NetworkManager.CurrentPort = (ushort)_port;
        if (NetworkManager.MaxPlayers != _maxConnections) NetworkManager.MaxPlayers = (ushort)_maxConnections;
        if (NetworkManager.CurrentIP != _ip) NetworkManager.CurrentIP = _ip;
        
        CopperImGui.Text("IP", ref _ip);
        CopperImGui.DragValue("Port", ref _port);
        CopperImGui.DragValue("Max Connections", ref _maxConnections);
        
        if (NetworkManager.CurrentServer == null)
        {
            CopperImGui.Button("Start Server", () => NetworkManager.StartServer());
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