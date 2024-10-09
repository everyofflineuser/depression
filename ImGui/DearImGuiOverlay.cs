using CopperDevs.DearImGui;
using CopperDevs.DearImGui.Renderer.Raylib;
using Newtonsoft.Json;
using Raylib_CSharp.Logging;
using Sparkle_Editor.Code.Managers;
using Sparkle.CSharp.Overlays;
using Sparkle.CSharp.Scenes;
using Logger = Sparkle.CSharp.Logging.Logger;

namespace Sparkle_Editor.Code.ImGui;

public class DearImGuiOverlay : Overlay
{
    public DearImGuiOverlay(string name) : base(name)
    {
        CopperImGui.Setup<RlImGuiRenderer>(true, true);
        CopperImGui.ShowDearImGuiAboutWindow = false;
        CopperImGui.ShowDearImGuiDemoWindow = true;
        CopperImGui.ShowDearImGuiMetricsWindow = false;
        CopperImGui.ShowDearImGuiDebugLogWindow = false;
        CopperImGui.ShowDearImGuiIdStackToolWindow = false;
    }

    protected override void Draw()
    {
        CopperImGui.Render();
    }

    ~DearImGuiOverlay()
    {
        CopperImGui.Shutdown();
    }
}