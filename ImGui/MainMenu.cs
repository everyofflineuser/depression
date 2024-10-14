using CopperDevs.DearImGui;
using CopperDevs.DearImGui.Attributes;
using CopperDevs.DearImGui.Rendering;

namespace depression.ImGui;

[Window("Main Menu", WindowOpen = true)]
public class MainMenu : BaseWindow
{
    private MenuState _state = 0;

    public override void WindowUpdate()
    {
        base.WindowUpdate();
        
        
    }
}

internal enum MenuState
{
    Normal = 0,
    Multiplayer = 1
}