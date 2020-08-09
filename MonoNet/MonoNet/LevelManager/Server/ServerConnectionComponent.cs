using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Network;
using MonoNet.Network.Commands;
using MonoNet.Network.MasterServerConnection;
using MonoNet.PickUps;
using MonoNet.Player;
using MonoNet.Util;
using System;

namespace MonoNet.LevelManager
{
    public class ServerConnectionComponent : Component, IDisposable
    {
        public static ServerConnectionComponent Instance { get; private set; }

        private PlayerSpawnLocations spawnLocations;
        private Stage stage;
        private string name;

        private int playersConnected = 1;

        protected override void OnInitialize()
        {
            Instance = this;

            NetManager.Instance.OnPlayerConnected += OnPlayerConnected;
            NetManager.Instance.OnPlayerDisconnected += OnPlayerDisconnected;

            GameManager.Instance.OnGameEnd += OnGameEnd;
        }

        public void Set(PlayerSpawnLocations spawnLocations, string name, Stage stage)
        {
            this.spawnLocations = spawnLocations;
            this.name = name;
            this.stage = stage;
        }

        public void OnPlayerConnected(ConnectedClient client)
        {
            playersConnected++;
            MasterServerConnector.Instance.UpdatePlayerCount(playersConnected);

            Log.Info("Player connected!");
            if (GameManager.Instance.RoundStarted == true)
            {
                NetSyncComponent.TriggerClientEvent(client, "WR");
                client.waiting = true;
            }
            else
            {
                client.waiting = false;
                client.hasChangedLevel = false;
                NetSyncComponent.TriggerClientEvent(client, "CL", GameManager.currentLevel);
            }
        }

        public void CreatePlayer(ConnectedClient client)
        {
            byte id = CreateSyncable((byte)GameManager.playerLayer, out NetSyncComponent nsc);
            Vector2 vector = CreateNetPlayer(id, client.name);
            NetSyncComponent.TriggerClientEvent(client, "TC", nsc.Id, vector);
            client.controlledComponents.Add(nsc);
        }

        [EventHandler("D")]
        public void Disconnect(byte playerId)
        {
            NetManager.Instance.GetClient(playerId).hasExited = true;
        }

        public void Shutdown()
        {
            NetSyncComponent.TriggerClientEvent("SS");
        }

        public void OnPlayerDisconnected(ConnectedClient client)
        {
            playersConnected--;
            MasterServerConnector.Instance.UpdatePlayerCount(playersConnected);

            Log.Info("Player disconnected!");
            for (int i = 0; i < client.controlledComponents.Count; i++)
                DestroySyncable(client.controlledComponents[i].Id);
        }

        public byte CreateSyncable(byte layer, out NetSyncComponent ncs)
        {
            Actor actor = Instance.stage.CreateActor(layer);
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
            PlayerManager player = NetManagerSender.Instance.GetClient(playerId).controlledComponents[0].Actor.GetComponent<PlayerManager>();
            if (player.Equip.ActiveWeapon?.CanShoot != true)
            {
                return;
            }

            player.lookingAt = (PlayerManager.LookingAt)direction;
            CreateBullet(player);
        }

        public void CreateWeapon(byte weaponId, Vector2 position)
        {
            CreateSyncable((byte)GameManager.playerLayer, out NetSyncComponent ncs);
            NetSyncComponent.TriggerClientEvent("CW", ncs.Id, weaponId);
            ComponentFactory.CreateWeapon(ncs.Id, weaponId);
            ncs.Actor.GetComponent<Transform2>().WorldPosition = position;
        }

        public void CreateBullet(PlayerManager player)
        {
            byte id = CreateSyncable((byte)GameManager.bulletLayer, out NetSyncComponent ncs);
            Actor bulletActor = ComponentFactory.CreatePreparedBullet(id);
            bulletActor.GetComponent<Transform2>().WorldPosition = player.Actor.GetComponent<Transform2>().WorldPosition + player.GetLookingVector();

            NetSyncComponent.TriggerClientEvent("CB", id);

            Weapon weapon = player.Equip.ActiveWeapon;
            Bullet bullet = bulletActor.AddComponent<Bullet>();

            bullet.Set(weapon.BulletVelocity, player);
        }

        [EventHandler("RW")]
        public void RequestWeaponPickup(byte playerId, byte weaponId)
        {
            PlayerManager player = NetManagerSender.Instance.GetClient(playerId).controlledComponents[0].Actor.GetComponent<PlayerManager>();
            Weapon weapon = NetManager.Instance.GetNetSyncComponent(weaponId).Actor.GetComponent<Weapon>();

            if (player.CanPickUp(out _, weapon) == false)
                return;

            NetSyncComponent.TriggerClientEvent(NetManager.Instance.GetClient(playerId), "PW", player.Actor.GetComponent<NetSyncComponent>().Id, weaponId);
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

            child.Parent = parent;
        }

        public void ChangeLevel(byte levelId)
        {
            GameManager.currentLevel = levelId;
            NetSyncComponent.TriggerClientEvent("CL", levelId);
            for (int i = 0; i < NetManager.Instance.ConnectedAdresses.Count; i++)
            {
                NetManager.Instance.ConnectedAdresses[i].hasChangedLevel = false;
                NetManager.Instance.ConnectedAdresses[i].waiting = false;
                NetManager.Instance.ConnectedAdresses[i].controlledComponents.Clear();
            }
            NetManager.Instance.Reset();

            LevelScreen.Instance.RequestLevelChange(levelId);
            stage.CreateActor(0).AddComponent<HostGameStartComponent>();
        }

        public void StartGame()
        {
            spawnLocations.Randomize();
            GameManager.Instance.RoundStarted = true;
            for (int i = 0; i < NetManager.Instance.ConnectedAdresses.Count; i++)
            {
                CreatePlayer(NetManager.Instance.ConnectedAdresses[i]);
            }
            byte id = CreateSyncable((byte)GameManager.playerLayer, out NetSyncComponent nsc);
            Vector2 vector = CreateNetPlayer(id, name);
            nsc.Actor.AddComponent<PlayerInput>();
            nsc.Actor.GetComponent<Transform2>().WorldPosition = vector;
            nsc.Actor.GetComponent<Rigidbody>().velocity = new Vector2(0);
        }

        public void OnGameEnd(PlayerManager player)
        {
            if (player != null)
                NetSyncComponent.TriggerClientEvent("GE", false, player.name);
            else
                NetSyncComponent.TriggerClientEvent("GE", true, "a");
        }

        [EventHandler("LL")]
        public void LevelLoaded(byte playerId)
        {
            NetManager.Instance.GetClient(playerId).hasChangedLevel = true;
        }

        /*
        public void JoinSelf()
        {
            CreateSyncable((byte)GameManager.playerLayer, out NetSyncComponent ncs);
            CreateNetPlayer(ncs.Id, "Server");
            ncs.Actor.AddComponent<PlayerInput>();
        }
        */

        public void DestroySyncable(byte netId)
        {
            NetSyncComponent nsc = NetManager.Instance.GetNetSyncComponent(netId);

            if (nsc == null)
                return;

            nsc.Actor.Stage.DeleteActor(nsc.Actor);
            NetManager.Instance.SetNetSyncComponent(null, netId);

            NetSyncComponent.TriggerClientEvent("DS", netId);
        }

        public void Dispose()
        {
            NetManager.Instance.OnPlayerConnected -= OnPlayerConnected;
            NetManager.Instance.OnPlayerDisconnected -= OnPlayerDisconnected;
        }
    }
}
