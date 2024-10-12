﻿using System.Numerics;
using Raylib_CSharp.Interact;
using Riptide;
using Sparkle_Editor.Code.Managers;
using Sparkle.CSharp.Entities;

namespace Sparkle_Editor.Code.Entities;

public abstract class NetworkEntity : Entity
{
    protected NetworkEntity(Vector3 position) : base(position)
    {
        NetworkManager.AddNetworkEntity(this);
    }

    protected override void Update()
    {
        base.Update();
        
        if (Input.IsKeyPressed(KeyboardKey.C))
        {
            this.Position.X++;
            this.Position.Y++;
            this.Position.Z++;
            
            Message entityUpdate = Message.Create(MessageSendMode.Reliable, MessageId.EntityUpdate)
                .AddVector3(this.Position)
                .AddQuaternion(this.Rotation)
                .AddUShort((ushort)this.Id);
            
            NetworkManager.SendMessage(entityUpdate);
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        NetworkManager.RemoveNetworkEntity(this);
    }
}