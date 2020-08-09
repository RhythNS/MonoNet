using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.GameSystems.PickUps;
using MonoNet.Graphics;
using MonoNet.LevelManager;
using MonoNet.Network;
using MonoNet.PickUps;
using System;
using System.Collections.Generic;

namespace MonoNet.Player
{
    public delegate void OnDeath();

    public class PlayerManager : Component, ISyncable, IDisposable
    {
        public event OnDeath OnDeath;
        public string name = "Unknown";

        public PlayerInput PlayerInput { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public Equip Equip { get; private set; }
        public DrawTextureRegionComponent DrawComponent { get; private set; }

        public float XSpeed { get; private set; } = 150;
        public float XMaxSpeed { get; private set; } = 350;

        public enum LookingAt : byte
        {
            Right, Left, Up
        }

        public float JumpForce { get; private set; } = 250;
        public bool Dead { get; private set; }

        public int Health { get; private set; } = 3;

        public LookingAt lookingAt;
        private PlayerKeys binding;

        protected override void OnInitialize()
        {
            Equip = Actor.AddComponent<Equip>();
            PlayerInput = Actor.AddComponent<PlayerInput>();
            Rigidbody = Actor.GetComponent<Rigidbody>();
            DrawComponent = Actor.GetComponent<DrawTextureRegionComponent>();
            GameManager.RegisterPlayer(this);
        }

        public void TakeDamage()
        {
            Health--;
            if (Health <= 0)
            {
                Dead = true;

                ServerConnectionComponent.Instance.DestroySyncable(Actor.GetComponent<NetSyncComponent>().Id);
            }
        }

        public Vector2 GetLookingVector()
        {
            if (lookingAt == LookingAt.Right)
            {
                return Vector2.UnitX;
            }
            else if (lookingAt == LookingAt.Up)
            {
                return Vector2.UnitY * -1;
            }
            else
            {
                return Vector2.UnitX * -1;
            }
        }

        public void Sync(byte[] data, ref int pointerAt)
        {
            lookingAt = (LookingAt)NetUtils.GetNextByte(data, ref pointerAt);
        }

        public void GetSync(List<byte> data)
        {
            NetUtils.AddByteToList((byte)lookingAt, data);
        }

        public bool CanPickUp(out Pickable pickable, Pickable specificPickup = null)
        {
            Rigidbody[] overlapingBodies = Rigidbody.GetOverlaps();
            pickable = null;
            for (int i = 0; i < overlapingBodies.Length; i++)
            {
                if (overlapingBodies[i].Actor.TryGetComponent(out Pickable tryPick) == true &&
                    (specificPickup == null || specificPickup == tryPick))
                {
                    pickable = tryPick;
                    break;
                }
            }

            if (pickable is null)
                return false;

            // someone already picked it up
            if (pickable.Actor.GetComponent<Transform2>().Parent != null)
                return false;

            return true;
        }

        public void PickUp(Pickable pickable)
        {
            if (pickable is Weapon weapon)
                Equip.PickupWeapon(weapon);
            if (pickable is PickUp powerup)
                Equip.RegisterPowerUP(powerup);
        }

        public void Dispose()
        {
            GameManager.DeRegisterPlayer(this);
        }
    }
}
