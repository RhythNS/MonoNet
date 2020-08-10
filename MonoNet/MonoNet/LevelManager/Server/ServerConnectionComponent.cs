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
    /// <summary>
    /// Holds all methods that the client can call with rpcs and all methods that call
    /// methods from the client as rpcs.
    /// </summary>
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

        /// <summary>
        /// Sets important references of the ServerConnectionComponent.
        /// </summary>
        /// <param name="spawnLocations">The spawn locations of each player.</param>
        /// <param name="name">The name of the server.</param>
        /// <param name="stage">A reference to the main stage.</param>
        public void Set(PlayerSpawnLocations spawnLocations, string name, Stage stage)
        {
            this.spawnLocations = spawnLocations;
            this.name = name;
            this.stage = stage;
        }

        /// <summary>
        /// Called when a player connects. Lets the player wait until the next round starts or lets the
        /// player load the current map.
        /// </summary>
        /// <param name="client">The client that just connected.</param>
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

        /// <summary>
        /// Creates a player for a ConnectedClient.
        /// </summary>
        /// <param name="client">The client that owns the player.</param>
        public void CreatePlayer(ConnectedClient client)
        {
            byte id = CreateSyncable((byte)GameManager.playerLayer, out NetSyncComponent nsc);
            Vector2 vector = CreateNetPlayer(id, client.name);
            NetSyncComponent.TriggerClientEvent(client, "TC", nsc.Id, vector);
            client.controlledComponents.Add(nsc);
        }

        /// <summary>
        /// Callback when a client disconnects.
        /// </summary>
        /// <param name="playerId">The id of the quitting player.</param>
        [EventHandler("D")]
        public void Disconnect(byte playerId)
        {
            NetManager.Instance.GetClient(playerId).hasExited = true;
        }

        /// <summary>
        /// Called when the server should shutdown. Notifies all connected clients.
        /// </summary>
        public void Shutdown()
        {
            NetSyncComponent.TriggerClientEvent("SS");
        }

        /// <summary>
        /// Called when a player disconnects and deletes all Components that the client owned.
        /// </summary>
        /// <param name="client">The quitting client.</param>
        public void OnPlayerDisconnected(ConnectedClient client)
        {
            playersConnected--;
            MasterServerConnector.Instance.UpdatePlayerCount(playersConnected);

            Log.Info("Player disconnected!");
            for (int i = 0; i < client.controlledComponents.Count; i++)
                DestroySyncable(client.controlledComponents[i].Id);
        }

        /// <summary>
        /// Creates a new syncable that is synchronized between all players.
        /// </summary>
        /// <param name="layer">The layer to where the actor is placed on.</param>
        /// <param name="ncs">A reference to the NetSyncComponent.</param>
        /// <returns>The id of the NetSyncComponent</returns>
        public byte CreateSyncable(byte layer, out NetSyncComponent ncs)
        {
            Actor actor = Instance.stage.CreateActor(layer);
            ncs = actor.AddComponent<NetSyncComponent>();

            NetSyncComponent.TriggerClientEvent("CS", ncs.Id, layer);

            return ncs.Id;
        }

        /// <summary>
        /// Creates a NetPlayer for a client when a round restarts.
        /// </summary>
        /// <param name="netId">The id of the player.</param>
        /// <param name="name">The name of the player.</param>
        /// <returns>The position of the newly created player.</returns>
        public Vector2 CreateNetPlayer(byte netId, string name)
        {
            PlayerManager player = ComponentFactory.CreateNetPlayer(netId, name);

            NetSyncComponent.TriggerClientEvent("CP", netId, name);

            Vector2 location = spawnLocations.GetRandomLocation();
            player.Actor.GetComponent<Transform2>().WorldPosition = location;

            return location;
        }

        /// <summary>
        /// Callback when a player wants to create a bullet.
        /// </summary>
        /// <param name="playerId">The client id of the player.</param>
        /// <param name="direction">The direction the player is facing.</param>
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

        /// <summary>
        /// Spawns a new weapon.
        /// </summary>
        /// <param name="weaponId">The weaponId of the newly created weapon.</param>
        /// <param name="position">The position to where the weapon should spawn.</param>
        public void CreateWeapon(byte weaponId, Vector2 position)
        {
            CreateSyncable((byte)GameManager.playerLayer, out NetSyncComponent ncs);
            NetSyncComponent.TriggerClientEvent("CW", ncs.Id, weaponId);
            ComponentFactory.CreateWeapon(ncs.Id, weaponId);
            ncs.Actor.GetComponent<Transform2>().WorldPosition = position;
        }
       
        /// <summary>
        /// Creates a bullet for all players.
        /// </summary>
        /// <param name="player">The player who shot the bullet.</param>
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

        /// <summary>
        /// Callback when a player wants to pick up a weapon.
        /// </summary>
        /// <param name="playerId">The player who wants to pick up a weapon.</param>
        /// <param name="weaponId">The netsync id of the weapon.</param>
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

        /// <summary>
        /// Callback when a player wants to drop a weapon.
        /// </summary>
        /// <param name="playerId">The id of the client.</param>
        [EventHandler("RD")]
        public void RequestDropWeapon(byte playerId)
        {
            NetManager.Instance.GetNetSyncComponent(playerId).Actor.GetComponent<PlayerManager>().Equip.DropWeapon();
        }

        /// <summary>
        /// Called when a transform gets a new parent.
        /// </summary>
        /// <param name="child">The child Transform.</param>
        /// <param name="parent">The parent transform.</param>
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

        /// <summary>
        /// Changes the level on each conneted client and this server.
        /// </summary>
        /// <param name="levelId">The new level that should loaded.</param>
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

        /// <summary>
        /// Starts the game for each connected client.
        /// </summary>
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

        /// <summary>
        /// Called when the game ends.
        /// </summary>
        /// <param name="player"></param>
        public void OnGameEnd(PlayerManager player)
        {
            if (player != null)
                NetSyncComponent.TriggerClientEvent("GE", false, player.name);
            else
                NetSyncComponent.TriggerClientEvent("GE", true, "a");
        }

        /// <summary>
        /// Callback when a client finished loading a level.
        /// </summary>
        /// <param name="playerId">The id of the client that loaded a level.</param>
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

        /// <summary>
        /// Destroyes a syncable for each client.
        /// </summary>
        /// <param name="netId">The id of the NetSyncComponent that should be deleted.</param>
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
