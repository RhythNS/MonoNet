using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Network;
using MonoNet.Network.Commands;
using MonoNet.Player;
using System;

namespace MonoNet.LevelManager
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
        public void CreateNetPlayer(byte netId, string name)
        {
            TextureRegion textureRegion = Instance.textureRegion;
            Actor actor = NetManager.Instance.GetNetSyncComponent(netId).Actor;
            Transform2 trans = actor.AddComponent<Transform2>();
            Rigidbody body = actor.AddComponent<Rigidbody>();
            actor.AddComponent<PlayerManager>().name = name;
            actor.RemoveComponent<PlayerInput>();
            body.Set(width: textureRegion.sourceRectangle.Width, height: textureRegion.sourceRectangle.Height, collisionLayer: 1, isStatic: false, isSquare: true, isTrigger: false);
            DrawTextureRegionComponent drawTexture = actor.AddComponent<DrawTextureRegionComponent>();
            drawTexture.region = textureRegion;
        }

        [EventHandler("CB")]
        public void CreateBullet(byte playerId)
        {
            //TODO: Implement
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

        [EventHandler("PW")]
        public void PickupWeapon(byte weaponId)
        {
            //TODO: Implement
        }

        [EventHandler("DS")]
        public void DestroySyncable(byte netId)
        {
            NetSyncComponent nsc = NetManager.Instance.GetNetSyncComponent(netId);
            nsc.Actor.Stage.DeleteActor(nsc.Actor);
            NetManager.Instance.SetNetSyncComponent(null, netId);
        }

        [EventHandler("RL")]
        public void ReloadLevel(byte levelNumber)
        {
            //TODO: Implement
        }

        [EventHandler("PT")]
        public void ParentTransform(byte childNumber, byte parentNumber)
        {
            // TODO: Implement
            // if childNumber == parentNumber then child is null
        }

    }
}
