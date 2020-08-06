using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Network;
using MonoNet.Player;
using MonoNet.Testing.Infrastructure;
using MonoNet.Util;

namespace MonoNet.Testing.NetTest
{
    public class ServerTestComponent : Component
    {
        private PlayerSpawnLocations spawnLocations;
        private TextureRegion textureRegion;

        protected override void OnInitialize()
        {
            NetManager.Instance.OnPlayerConnected += OnPlayerConnected;
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
        
    }
}
