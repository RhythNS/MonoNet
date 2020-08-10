using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Network;
using MonoNet.Network.Commands;
using MonoNet.PickUps;
using MonoNet.Player;
using Myra.Graphics2D.UI.ColorPicker;
using System;

namespace MonoNet.LevelManager
{
    public class ClientConnectionComponent : Component
    {
        public static ClientConnectionComponent Instance { get; private set; }
        public ClientLevelScreen clientLevelScreen;

        protected override void OnInitialize()
        {
            Instance = this;
        }

        /// <summary>
        /// Sets all required references.
        /// </summary>
        /// <param name="clientLevelScreen">A refernce to the clientLevelScreen.</param>
        public void Set(ClientLevelScreen clientLevelScreen)
        {
            this.clientLevelScreen = clientLevelScreen;
        }
        
        /// <summary>
        /// Callback function when the server wants to create a new Syncable.
        /// </summary>
        /// <param name="id">The id of the newly created NetSync.</param>
        /// <param name="layer">The layer on which the actor is placed.</param>
        [EventHandler("CS")]
        public void CreateSyncable(byte id, byte layer)
        {
            Actor actor = Instance.Actor.Stage.CreateActor(layer);
            actor.AddComponent<NetSyncComponent>().Id = id;
        }

        /// <summary>
        /// Callback function when a new player joined the game.
        /// </summary>
        /// <param name="netId">The NetSync id of the new player.</param>
        /// <param name="name">The name of the player.</param>
        [EventHandler("CP")]
        public void CreateNetPlayer(byte netId, string name)
        {
            ComponentFactory.CreateNetPlayer(netId, name);
        }

        /// <summary>
        /// Callback function when the server spawns a new weapon.
        /// </summary>
        /// <param name="netId">The NetSync id of the new weapon.</param>
        /// <param name="weaponId">The weaponId of the newly created weapon.</param>
        [EventHandler("CW")]
        public void CreateWeapon(byte netId, byte weaponId)
        {
            ComponentFactory.CreateWeapon(netId, weaponId);
        }

        /// <summary>
        /// Callback function when the server spawns a new bullet.
        /// </summary>
        /// <param name="id">The NetSync id of the new bullet.</param>
        [EventHandler("CB")]
        public void CreateBullet(byte id)
        {
            ComponentFactory.CreatePreparedBullet(id);
        }

        /// <summary>
        /// Requests a bullet to spawn on the server.
        /// </summary>
        /// <param name="player">A reference to the player the client controlls.</param>
        public void RequestBullet(PlayerManager player)
        {
            NetSyncComponent.TriggerServerEvent("RB", false, (byte)player.lookingAt);
        }

        /// <summary>
        /// Takes control of a PlayerManager.
        /// </summary>
        /// <param name="netId">The NetSync Id of the playerManager to control.</param>
        /// <param name="location">The location where the player spawns.</param>
        [EventHandler("TC")]
        public void TakeControl(byte netId, Vector2 location)
        {
            NetSyncComponent nsc = NetManager.Instance.GetNetSyncComponent(netId);
            nsc.playerControlled = true;
            nsc.Actor.AddComponent<PlayerInput>();
            nsc.Actor.GetComponent<Transform2>().WorldPosition = location;
            nsc.Actor.GetComponent<Rigidbody>().velocity = new Vector2(0);

            LevelUI.DisplayString("");
        }

        /// <summary>
        /// Callback function when a player picks up a weapon.
        /// </summary>
        /// <param name="netId">The netsync id of the player.</param>
        /// <param name="weaponId">The netsync id of the weapon.</param>
        [EventHandler("PW")]
        public void PickupWeapon(byte netId, byte weaponId)
        {
            NetSyncComponent playerNcs = NetManager.Instance.GetNetSyncComponent(netId);
            NetSyncComponent weaponNcs = NetManager.Instance.GetNetSyncComponent(weaponId);

            playerNcs.Actor.GetComponent<PlayerManager>().PickUp(weaponNcs.Actor.GetComponent<Weapon>());
        }

        /// <summary>
        /// Requests to pickup a weapon from the server.
        /// </summary>
        /// <param name="pickable">The weapon to be picked up.</param>
        public void RequestPickupWeapon(Pickable pickable)
        {
            NetSyncComponent.TriggerServerEvent("RW", false, pickable.Actor.GetComponent<NetSyncComponent>().Id);
        }

        /// <summary>
        /// Callback function when the server shutsdown.
        /// </summary>
        [EventHandler("SS")]
        public void ServerShutdown()
        {
            ((ClientLevelScreen)LevelScreen.Instance).OnDisconnect("Server shutdown!\nPress escape to go back to the main menu!");
        }

        /// <summary>
        /// Requests to drop the current weapon to the server.
        /// </summary>
        public void RequestWeaponDrop()
        {
            NetSyncComponent.TriggerServerEvent("RD", false);
        }

        /// <summary>
        /// Callback function when the server wants to destroy a syncable.
        /// </summary>
        /// <param name="netId">The netsync id to be destroyed.</param>
        [EventHandler("DS")]
        public void DestroySyncable(byte netId)
        {
            NetSyncComponent nsc = NetManager.Instance.GetNetSyncComponent(netId);
            nsc.Actor.Stage.DeleteActor(nsc.Actor);
            NetManager.Instance.SetNetSyncComponent(null, netId);
        }

        /// <summary>
        /// Callback function when the server wants to change the level.
        /// </summary>
        /// <param name="levelNumber"></param>
        [EventHandler("CL")]
        public void ChangeLevel(byte levelNumber)
        {
            Instance.clientLevelScreen.RequestLevelChange(levelNumber);
        }

        /// <summary>
        /// Tells the server that this client loaded the new level.
        /// </summary>
        public void LevelLoaded()
        {
            NetSyncComponent.TriggerServerEvent("LL", true);
            NetManager.Instance.Reset();
        }

        /// <summary>
        /// Callback function when a round is in progress.
        /// </summary>
        [EventHandler("WR")]
        public void WaitForRestart()
        {
            LevelUI.DisplayString("Game in progress.\nWaiting until the round ends!");
        }

        /// <summary>
        /// Callback function when the server wants to parent a Transform to another.
        /// </summary>
        /// <param name="childNumber">The netsync id of the child.</param>
        /// <param name="parentNumber">The netsync id of the parent.</param>
        [EventHandler("PT")]
        public void ParentTransform(byte childNumber, byte parentNumber)
        {
            Transform2 child = NetManager.Instance.GetNetSyncComponent(childNumber).Actor.GetComponent<Transform2>();

            if (childNumber == parentNumber)
            {
                child.Parent = null;
                if (child.Actor.TryGetComponent(out Weapon weapon))
                    weapon.holder.Equip.DropWeapon();
                return;
            }

            Transform2 parent = NetManager.Instance.GetNetSyncComponent(parentNumber).Actor.GetComponent<Transform2>();

            if (child.Actor.TryGetComponent(out Weapon pickedWeapon) && parent.Actor.TryGetComponent(out PlayerManager player))
                player.PickUp(pickedWeapon);
            child.Parent = parent;
        }

        /// <summary>
        /// Callback function when the round ends.
        /// </summary>
        /// <param name="draw">Wheter the round ended in a draw.</param>
        /// <param name="name">The name of the winning player.</param>
        [EventHandler("GE")]
        public void GameEnd(bool draw, string name)
        {
            Instance.clientLevelScreen.GameEnd(draw, name);
        }
    }
}
