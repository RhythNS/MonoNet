using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Network;
using MonoNet.Player;
using MonoNet.Testing.Infrastructure;
using MonoNet.Util;

namespace MonoNet.Testing.NetTest
{
    public class ServerTestComponent : Component, Interfaces.IUpdateable, IDisposable
    {
        private PlayerSpawnLocations spawnLocations;
        private TextureRegion textureRegion;

        protected override void OnInitialize()
        {
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
            Vector2 vector = CreateNetPlayer(id);
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

        public Vector2 CreateNetPlayer(byte netId)
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

        private bool alreadyJoinedSelf = false;
        public void JoinSelf()
        {
            CreateSyncable(2, out NetSyncComponent ncs);
            CreateNetPlayer(ncs.Id);
            ncs.Actor.AddComponent<PlayerInput>();
        }

        public void DestroySyncable(byte netId)
        {
            NetSyncComponent nsc = NetManager.Instance.GetNetSyncComponent(netId);
            nsc.Actor.Stage.DeleteActor(nsc.Actor);
            NetManager.Instance.SetNetSyncComponent(null, netId);

            NetSyncComponent.TriggerClientEvent("DS", netId);
        }

        public void AddRenderer(byte netId)
        {
            Actor actor = NetManager.Instance.GetNetSyncComponent(netId).Actor;
            actor.AddComponent<Transform2>();
            actor.AddComponent<DrawTextureRegionComponent>().region = textureRegion;
            actor.AddComponent<Rigidbody>().Set();

            NetSyncComponent.TriggerClientEvent("AR", netId);
        }

        static byte autoIncrementDestroy = 1;

        public void Update()
        {
            if (Input.IsKeyDownThisFrame(Keys.F5))
            {
                CreateSyncable(2, out NetSyncComponent ncs);
                AddRenderer(ncs.Id);
            }
            else if (Input.IsKeyDownThisFrame(Keys.F6))
            {
                DestroySyncable(autoIncrementDestroy++);
            }
            else if (Input.IsKeyDownThisFrame(Keys.F3))
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
