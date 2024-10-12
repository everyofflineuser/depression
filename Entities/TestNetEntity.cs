﻿using System.Numerics;
using Raylib_CSharp.Interact;
using Sparkle.CSharp.Entities.Components;
using Sparkle.CSharp.Logging;

namespace Sparkle_Editor.Code.Entities;

public class TestNetEntity : NetworkEntity
{
    public TestNetEntity(Vector3 position) : base(position)
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
    }
}