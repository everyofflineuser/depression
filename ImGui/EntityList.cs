using CopperDevs.DearImGui.Attributes;
using CopperDevs.DearImGui.Rendering;
using depression.Managers;
using ImGuiNET;
using Sparkle.CSharp.Scenes;

namespace depression.ImGui;

[Window("EntityList", WindowOpen = true)]
public class EntityList : BaseWindow
{
    public override void WindowUpdate()
    {
        base.WindowUpdate();
        
        if (ImGuiNET.ImGui.TreeNodeEx(SceneManager.ActiveScene?.Name, ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick))
        {
            foreach (var entity in NetworkManager.GetNetworkEntities())
            {
                if (entity == null || entity.HasDisposed || entity.Id == 1) continue;
                
                ImGuiTreeNodeFlags flag = ImGuiTreeNodeFlags.Leaf;
                
                string name = $"{entity.Id}: Entity";
                
                if (!string.IsNullOrEmpty(entity.Tag))
                {
                    name = $"{entity.Id}: {entity.Tag}";
                }
                
                if (ImGuiNET.ImGui.TreeNodeEx(name, flag))
                {
                    ImGuiNET.ImGui.TreePop();
                }
            }
        }
    }
}