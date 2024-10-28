using System.Numerics;
using Raylib_CSharp.Interact;
using Sparkle.CSharp.Entities.Components;
using Sparkle.CSharp.Logging;

namespace depression.Entities;

public class TestNetEntity : NetworkEntity
{
    public TestNetEntity(Vector3 position) : base(position)
    {
    }
    
    public TestNetEntity() : base(Vector3.Zero)
    {
    }

    protected override void Init()
    {
        base.Init();
        
        AddComponent(new ModelRenderer(ContentRegistry.Models["Cube"], Vector3.Zero));
    }

    protected override void Update()
    {
        base.Update();

        if (Input.IsKeyPressed(KeyboardKey.E))
        {
            Logger.Info($"TestNetEntity: {Position}");
        }
        
        if (Input.IsKeyPressed(KeyboardKey.C))
        {
            this.Position.X++;
            this.Position.Y++;
            this.Position.Z++;
        }
        
        if (Input.IsKeyPressed(KeyboardKey.F))
        {
            this.Scale.X++;
            this.Scale.Y++;
            this.Scale.Z++;
        }
    }
}