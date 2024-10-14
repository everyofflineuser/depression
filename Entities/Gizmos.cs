using System.Numerics;
using depression.Extensions;
using depression.Managers;
using Raylib_CSharp.Interact;
using Riptide;
using Sparkle.CSharp.Entities;
using Sparkle.CSharp.Entities.Components;

namespace depression.Entities;

public class Gizmos : Entity
{
    public Gizmos(Vector3 position) : base(position) { }
    
    protected override void Init()
    {
        base.Init();
        
        AddComponent(new ModelRenderer(ContentRegistry.Models["Gizmos"], Vector3.Zero));
    }

    protected override void Update()
    {
        base.Update();

        if (Input.IsKeyPressed(KeyboardKey.B))
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
}