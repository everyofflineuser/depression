using System.Numerics;
using depression.Extensions;
using depression.Managers;
using Raylib_CSharp.Interact;
using Riptide;
using Sparkle.CSharp.Entities;

namespace depression.Entities;

public abstract class NetworkEntity : Entity
{
    private Vector3 LastSyncedPosition;
    private Vector3 LastSyncedScale;
    private Quaternion LastSyncedRotation;
    
    protected NetworkEntity(Vector3 position) : base(position)
    {
        NetworkManager.AddNetworkEntity(this);
        LastSyncedPosition = position;
        LastSyncedRotation = Rotation;
        LastSyncedScale = Scale;
    }

    protected override void Update()
    {
        base.Update();

        if (LastSyncedPosition != Position ||
            LastSyncedScale != Scale ||
            LastSyncedRotation != Rotation)

        {
            Message entityUpdate = Message.Create(MessageSendMode.Reliable, MessageId.EntityUpdate)
                .AddVector3(this.Position)
                .AddVector3(this.Scale)
                .AddQuaternion(this.Rotation)
                .AddUShort((ushort)this.Id);
            
            NetworkManager.SendMessage(entityUpdate);
            
            LastSyncedPosition = Position;
            LastSyncedScale = Scale;
            LastSyncedRotation = Rotation;
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        NetworkManager.RemoveNetworkEntity(this);
    }
}