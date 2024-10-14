using System.Numerics;
using CopperDevs.DearImGui;
using depression.ImGui;
using Sparkle.CSharp.Entities;
using Sparkle.CSharp.Physics;
using Sparkle.CSharp.Scenes;

namespace depression.Scenes;

public class MainMenu : Scene
{
    public MainMenu(string name = "Main Menu", SceneType type = SceneType.Scene2D, Simulation? simulation = null) : base(name, type, simulation)
    {
    }

    protected override void Init()
    {
        base.Init();
        
        Cam2D cam2D = new(Vector2.Zero, Vector2.Zero, Cam2D.CameraFollowMode.Normal);
        
        AddEntity(cam2D);
    }
}