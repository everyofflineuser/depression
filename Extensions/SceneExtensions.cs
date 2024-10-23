using depression.Entities;
using depression.Managers;
using Riptide;
using Sparkle.CSharp.Scenes;

namespace depression.Extensions;

public static class SceneExtensions
{
    public static void AddNetworkEntity(this Scene scene, NetworkEntity networkEntity)
    {
        if (NetworkManager.CurrentServer == null) return;

        scene.AddEntity(networkEntity);
        
        Message entityCreate = Message.Create(MessageSendMode.Reliable, MessageId.EntityCreate)
            .AddString(networkEntity.GetType().AssemblyQualifiedName)
            .AddVector3(networkEntity.Position)
            .AddVector3(networkEntity.Scale)
            .AddQuaternion(networkEntity.Rotation);
        
        MessageExtensions.SerializeEntityProperties(networkEntity, entityCreate);
        
        NetworkManager.CurrentServer.SendToAll(entityCreate);
    }
}