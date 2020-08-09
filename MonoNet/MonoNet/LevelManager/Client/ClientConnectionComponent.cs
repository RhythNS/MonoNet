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

        public void Set(ClientLevelScreen clientLevelScreen)
        {
            this.clientLevelScreen = clientLevelScreen;
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
            ComponentFactory.CreateNetPlayer(netId, name);
        }

        [EventHandler("CW")]
        public void CreateWeapon(byte netId, byte weaponId)
        {
            ComponentFactory.CreateWeapon(netId, weaponId);
        }

        [EventHandler("CB")]
        public void CreateBullet(byte id)
        {
            ComponentFactory.CreatePreparedBullet(id);
        }

        public void RequestBullet(PlayerManager player)
        {
            NetSyncComponent.TriggerServerEvent("RB", false, (byte)player.lookingAt);
        }

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

        [EventHandler("PW")]
        public void PickupWeapon(byte netId, byte weaponId)
        {
            NetSyncComponent playerNcs = NetManager.Instance.GetNetSyncComponent(netId);
            NetSyncComponent weaponNcs = NetManager.Instance.GetNetSyncComponent(weaponId);

            playerNcs.Actor.GetComponent<PlayerManager>().PickUp(weaponNcs.Actor.GetComponent<Weapon>());
        }

        public void RequestPickupWeapon(Pickable pickable)
        {
            NetSyncComponent.TriggerServerEvent("RW", false, pickable.Actor.GetComponent<NetSyncComponent>().Id);
        }

        [EventHandler("SS")]
        public void ServerShutdown()
        {
            ((ClientLevelScreen)LevelScreen.Instance).OnDisconnect("Server shutdown!\nPress escape to go back to the main menu!");
        }

        public void RequestWeaponDrop()
        {
            NetSyncComponent.TriggerServerEvent("RD", false);
        }

        [EventHandler("DS")]
        public void DestroySyncable(byte netId)
        {
            NetSyncComponent nsc = NetManager.Instance.GetNetSyncComponent(netId);
            nsc.Actor.Stage.DeleteActor(nsc.Actor);
            NetManager.Instance.SetNetSyncComponent(null, netId);
        }

        [EventHandler("CL")]
        public void ChangeLevel(byte levelNumber)
        {
            Instance.clientLevelScreen.RequestLevelChange(levelNumber);
        }

        public void LevelLoaded()
        {
            NetSyncComponent.TriggerServerEvent("LL", true);
            NetManager.Instance.Reset();
        }

        [EventHandler("WR")]
        public void WaitForRestart()
        {
            LevelUI.DisplayString("Game in progress.\nWaiting until the round ends!");
        }

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

        [EventHandler("GE")]
        public void GameEnd(bool draw, string name)
        {
            Instance.clientLevelScreen.GameEnd(draw, name);
        }
    }
}
