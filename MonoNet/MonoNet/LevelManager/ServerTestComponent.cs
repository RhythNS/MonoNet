using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Network;
using MonoNet.Network.Commands;
using MonoNet.Player;
using MonoNet.Testing.NetTest;
using MonoNet.Util;

namespace MonoNet.LevelManager
{
    public class ServerTestComponent : Component, Interfaces.IUpdateable, IDisposable
    {
        public static ServerTestComponent Instance { get; private set; }

        private PlayerSpawnLocations spawnLocations;
        private TextureRegion textureRegion;

        protected override void OnInitialize()
        {
            Instance = this;

            NetManager.Instance.OnPlayerConnected += OnPlayerConnected;
            NetManager.Instance.OnPlayerDisconnected += OnPlayerDisconnected;
        }

        public void Set(PlayerSpawnLocations spawnLocations, TextureRegion region)
        {
            this.spawnLocations = spawnLocations;
            textureRegion = region;
        }

        public void OnPlayerConnected(ConnectedClient client)
        {
            Log.Info("Player connected!");
            byte id = CreateSyncable(2, out NetSyncComponent ncs);
            Vector2 vector = CreateNetPlayer(id, client.name);
            NetSyncComponent.TriggerClientEvent(client, "TC", id, vector);
            client.controlledComponents.Add(ncs);
        }

        public void OnPlayerDisconnected(ConnectedClient client)
        {
            Log.Info("Player disconnected!");
            for (int i = 0; i < client.controlledComponents.Count; i++)
                DestroySyncable(client.controlledComponents[i].Id);
        }

        public byte CreateSyncable(byte layer, out NetSyncComponent ncs)
        {
            Actor actor = Actor.Stage.CreateActor(layer);
            ncs = actor.AddComponent<NetSyncComponent>();

            NetSyncComponent.TriggerClientEvent("CS", ncs.Id, layer);

            return ncs.Id;
        }

        public Vector2 CreateNetPlayer(byte netId, string name)
        {
            Actor actor = NetManager.Instance.GetNetSyncComponent(netId).Actor;
            Transform2 trans = actor.AddComponent<Transform2>();
            Rigidbody body = actor.AddComponent<Rigidbody>();
            actor.AddComponent<PlayerManager>();
            actor.RemoveComponent<PlayerInput>();
            body.Set(width: textureRegion.sourceRectangle.Width, height: textureRegion.sourceRectangle.Height, collisionLayer: 1, isStatic: false, isSquare: true, isTrigger: false);
            DrawTextureRegionComponent drawTexture = actor.AddComponent<DrawTextureRegionComponent>();
            drawTexture.region = textureRegion;

            NetSyncComponent.TriggerClientEvent("CP", netId);

            Vector2 location = spawnLocations.GetRandomLocation();
            trans.WorldPosition = location;

            return location;
        }

        [EventHandler("RB")]
        public void RequestBullet(byte playerId, byte direction)
        {
            //TODO: Implement
        }

        [EventHandler("RW")]
        public void RequestWeaponPickup(byte playerId, byte weaponId)
        {
            //TODO: Implement
            // Check if 
            // Trigger on all clients except guy who picked ParentTransform
            // trigger only on guy who picked PickupWeapon
        }

        private bool alreadyJoinedSelf = false;
        public void JoinSelf()
        {
            CreateSyncable(2, out NetSyncComponent ncs);
            CreateNetPlayer(ncs.Id, "Server");
            ncs.Actor.AddComponent<PlayerInput>();
        }

        public void DestroySyncable(byte netId)
        {
            NetSyncComponent nsc = NetManager.Instance.GetNetSyncComponent(netId);
            nsc.Actor.Stage.DeleteActor(nsc.Actor);
            NetManager.Instance.SetNetSyncComponent(null, netId);

            NetSyncComponent.TriggerClientEvent("DS", netId);
        }

        public void Update()
        {
            if (Input.IsKeyDownThisFrame(Keys.F3))
            {
                if (alreadyJoinedSelf == false)
                {
                    alreadyJoinedSelf = true;
                    JoinSelf();
                }
            }
        }

        public void Dispose()
        {
            NetManager.Instance.OnPlayerConnected -= OnPlayerConnected;
            NetManager.Instance.OnPlayerDisconnected -= OnPlayerDisconnected;
        }
    }
}
