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
using MonoNet.PickUps;
using MonoNet.Player;
using MonoNet.Util;

namespace MonoNet.LevelManager
{
    public class ServerConnectionComponent : Component, Interfaces.IUpdateable, IDisposable
    {
        public static ServerConnectionComponent Instance { get; private set; }

        private PlayerSpawnLocations spawnLocations;
        private Stage stage;

        protected override void OnInitialize()
        {
            Instance = this;

            NetManager.Instance.OnPlayerConnected += OnPlayerConnected;
            NetManager.Instance.OnPlayerDisconnected += OnPlayerDisconnected;
        }

        public void Set(PlayerSpawnLocations spawnLocations, Stage stage)
        {
            this.spawnLocations = spawnLocations;
            this.stage = stage;
        }

        public void OnPlayerConnected(ConnectedClient client)
        {
            Log.Info("Player connected!");
            CreatePlayer(client);
        }

        public void CreatePlayer(ConnectedClient client)
        {
            byte id = CreateSyncable((byte)GameManager.playerLayer, out NetSyncComponent nsc);
            Vector2 vector = CreateNetPlayer(id, client.name);
            NetSyncComponent.TriggerClientEvent(client, "TC", nsc.Id, vector);
            client.controlledComponents.Add(nsc);
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
            PlayerManager player = ComponentFactory.CreateNetPlayer(netId, name);

            NetSyncComponent.TriggerClientEvent("CP", netId, name);

            Vector2 location = spawnLocations.GetRandomLocation();
            player.Actor.GetComponent<Transform2>().WorldPosition = location;

            return location;
        }

        [EventHandler("RB")]
        public void RequestBullet(byte playerId, byte direction)
        {
            PlayerManager player = NetManager.Instance.GetNetSyncComponent(playerId).Actor.GetComponent<PlayerManager>();
            if (player.Equip.ActiveWeapon?.CanShoot != true)
            {
                return;
            }

            player.lookingAt = (PlayerManager.LookingAt)direction;
            CreateBullet(player);
        }

        public void CreateBullet(PlayerManager player)
        {
            byte id = CreateSyncable((byte)GameManager.bulletLayer, out NetSyncComponent ncs);
            Actor bulletActor = ComponentFactory.CreatePreparedBullet(id);

            NetSyncComponent.TriggerClientEvent("CB", id);

            Weapon weapon = player.Equip.ActiveWeapon;
            Bullet bullet = bulletActor.AddComponent<Bullet>();

            bullet.Set(weapon.BulletVelocity, player);
        }

        [EventHandler("RW")]
        public void RequestWeaponPickup(byte playerId, byte weaponId)
        {
            PlayerManager player = NetManager.Instance.GetNetSyncComponent(playerId).Actor.GetComponent<PlayerManager>();
            Weapon weapon = NetManager.Instance.GetNetSyncComponent(weaponId).Actor.GetComponent<Weapon>();

            if (player.CanPickUp(out _, weapon) == false)
                return;

            NetSyncComponent.TriggerClientEvent(NetManager.Instance.GetClient(playerId), "PW", playerId, weaponId);
            player.PickUp(weapon);
        }

        [EventHandler("RD")]
        public void RequestDropWeapon(byte playerId)
        {
            NetManager.Instance.GetNetSyncComponent(playerId).Actor.GetComponent<PlayerManager>().Equip.DropWeapon();
        }

        public void ParentTransform(Transform2 child, Transform2 parent)
        {
            byte childNumber = child.Actor.GetComponent<NetSyncComponent>().Id;
            byte parentNumber;
            if (parent == null)
                parentNumber = childNumber;
            else
                parentNumber = parent.Actor.GetComponent<NetSyncComponent>().Id;

            NetSyncComponent.TriggerClientEvent("PT", childNumber, parentNumber);

            child.Parent = null;
        }

        public void ChangeLevel(byte levelId)
        {
            NetSyncComponent.TriggerClientEvent("CL", levelId);
            for (int i = 0; i < NetManager.Instance.ConnectedAdresses.Count; i++)
                NetManager.Instance.ConnectedAdresses[i].hasChangedLevel = false;
            NetManager.Instance.Reset();

            LevelScreen.Instance.LoadLevel(levelId);
            stage.CreateActor(0).AddComponent<HostGameStartComponent>();
        }

        public void StartGame()
        {
            GameManager.Instance.RoundStarted = true;
            for (int i = 0; i < NetManager.Instance.ConnectedAdresses.Count; i++)
            {
                CreatePlayer(NetManager.Instance.ConnectedAdresses[i]);
            }
        }

        [EventHandler("LL")]
        public void LevelLoaded(byte playerId)
        {
            NetManager.Instance.GetClient(playerId).hasChangedLevel = true;
        }

        private bool alreadyJoinedSelf = false;
        public void JoinSelf()
        {
            CreateSyncable((byte)GameManager.playerLayer, out NetSyncComponent ncs);
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
