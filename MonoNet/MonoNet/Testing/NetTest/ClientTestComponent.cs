using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Network;
using MonoNet.Network.Commands;
using MonoNet.Player;
using System;

namespace MonoNet.Testing.NetTest
{
    public class ClientTestComponent : Component
    {
        public static ClientTestComponent Instance { get; private set; }

        private TextureRegion textureRegion;

        protected override void OnInitialize()
        {
            Instance = this;
        }

        public void Set(TextureRegion region)
        {
            textureRegion = region;
        }

        [EventHandler("CS")]
        public void CreateSyncable(byte id, byte layer)
        {
            Actor actor = Instance.Actor.Stage.CreateActor(layer);
            actor.AddComponent<NetSyncComponent>().Id = id;
        }

        [EventHandler("CP")]
        public void CreateNetPlayer(byte netId)
        {
            TextureRegion textureRegion = Instance.textureRegion;
            Actor actor = NetManager.Instance.GetNetSyncComponent(netId).Actor;
            Transform2 trans = actor.AddComponent<Transform2>();
            Rigidbody body = actor.AddComponent<Rigidbody>();
            actor.AddComponent<PlayerManager>();
            actor.RemoveComponent<PlayerInput>();
            body.Set(width: textureRegion.sourceRectangle.Width, height: textureRegion.sourceRectangle.Height, collisionLayer: 1, isStatic: false, isSquare: true, isTrigger: false);
            DrawTextureRegionComponent drawTexture = actor.AddComponent<DrawTextureRegionComponent>();
            drawTexture.region = textureRegion;
        }

        [EventHandler("TC")]
        public void TakeControl(byte netId, Vector2 location)
        {
            NetSyncComponent nsc = NetManager.Instance.GetNetSyncComponent(netId);
            nsc.playerControlled = true;
            nsc.Actor.AddComponent<PlayerInput>();
            nsc.Actor.GetComponent<Transform2>().WorldPosition = location;
            nsc.Actor.GetComponent<Rigidbody>().velocity = new Vector2(0);
        }

        [EventHandler("DS")]
        public void DestroySyncable(byte netId)
        {
            NetSyncComponent nsc = NetManager.Instance.GetNetSyncComponent(netId);
            nsc.Actor.Stage.DeleteActor(nsc.Actor);
            NetManager.Instance.SetNetSyncComponent(null, netId);
        }

        [EventHandler("AR")]
        public void AddRenderer(byte netId)
        {
            Actor actor = NetManager.Instance.GetNetSyncComponent(netId).Actor;
            actor.AddComponent<Transform2>();
            actor.AddComponent<DrawTextureRegionComponent>().region = Instance.textureRegion;
            actor.AddComponent<Rigidbody>().Set();
        }

    }
}
