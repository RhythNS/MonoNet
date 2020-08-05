using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Network;
using MonoNet.Network.Commands;
using MonoNet.Player;
using MonoNet.Testing.Infrastructure;
using System;
using System.Security;

namespace MonoNet.Testing.NetTest
{
    public class ClientTestComponent : Component
    {
        private TextureRegion textureRegion;

        public void Set(TextureRegion region)
        {
            textureRegion = region;
        }

        [EventHandler("CS")]
        public void CreateSyncable(byte layer)
        {
            Actor actor = Actor.Stage.CreateActor(layer);
            actor.AddComponent<NetSyncComponent>();
        }
        
        [EventHandler("CP")]
        public void CreateNetPlayer(byte netId)
        {
            Actor actor = NetManager.Instance.GetNetSyncComponent(netId).Actor;
            Transform2 trans = actor.AddComponent<Transform2>();
            actor.AddComponent<PlayerManager>();
            actor.RemoveComponent<PlayerInput>();
            Rigidbody body = actor.GetComponent<Rigidbody>();
            body.Set(width: textureRegion.sourceRectangle.Width, height: textureRegion.sourceRectangle.Height, collisionLayer: 1, isStatic: false, isSquare: true, isTrigger: false);
            DrawTextureRegionComponent drawTexture = actor.AddComponent<DrawTextureRegionComponent>();
            drawTexture.region = textureRegion;
        }
        
        [EventHandler("TC")]
        public void TakeControl(byte netId, Vector2 location)
        {
            NetSyncComponent nsc = NetManager.Instance.GetNetSyncComponent(netId);
            nsc.Actor.AddComponent<PlayerInput>();
            nsc.Actor.GetComponent<Transform2>().WorldPosition = location;
            nsc.Actor.GetComponent<Rigidbody>().velocity = new Vector2(0);
        }

    }
}
