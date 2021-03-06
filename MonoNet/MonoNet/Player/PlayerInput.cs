﻿using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.GameSystems.PickUps;
using MonoNet.LevelManager;
using MonoNet.Network;
using MonoNet.PickUps;
using System.Collections;

namespace MonoNet.Player
{
    public class PlayerInput : Component, Interfaces.IUpdateable
    {
        private PlayerManager player;
        private Rigidbody rigidbody;
        private PlayerKeys binding;
        private Transform2 transform;
        public Vector2 lastGround;
        private Equip equip;

        private Vector2 addVel;
        private bool movedRight;
        private bool jumping = false;
        private int jumpCount = 0;

        protected override void OnInitialize()
        {
            transform = Actor.GetComponent<Transform2>();
            player = Actor.GetComponent<PlayerManager>();
            rigidbody = Actor.GetComponent<Rigidbody>();
            equip = Actor.GetComponent<Equip>();
            binding = new PlayerKeys();
        }

        public void SetBinding(PlayerKeys keys)
        {
            binding = keys;
        }

        public void Update()
        {
            addVel = Vector2.Zero;

            Move();
            Jump();
            PickUp();
            Drop();
            Look();
            Shoot();

            addVel.X *= Time.Delta;
            rigidbody.velocity += addVel;
        }

        private void Move()
        {
            if (Input.KeyDown(binding.right)) // move right
            {
                if (movedRight == false)
                    StopMove();

                addVel.X += player.XSpeed;
                movedRight = true;

                if (rigidbody.velocity.X + addVel.X > player.XMaxSpeed)
                    addVel.X = 0;
            }
            else if (Input.KeyDown(binding.left)) // move left
            {
                if (movedRight == true)
                    StopMove();

                addVel.X -= player.XSpeed;
                movedRight = false;

                if (rigidbody.velocity.X + addVel.X < -player.XMaxSpeed)
                    addVel.X = 0;
            }
            //else // not moving
            else if (rigidbody.isGrounded == true) // not moving and grounded
            {
                StopMove();
            }

            if (rigidbody.isGrounded == true)
            {
                lastGround = transform.WorldPosition;
            }
        }

        private void StopMove()
        {
            rigidbody.velocity.X = 0;
        }

        private void Jump()
        {
            if (rigidbody.isGrounded == true && jumping == true)
            {
                StartCoroutine(StackJump());
            }

            if (rigidbody.isGrounded == true)
            {
                jumping = false;
                if (Input.IsKeyDownThisFrame(binding.jump))
                {
                    jumping = true;
                    addVel.Y -= player.JumpForce * (1 + 0.2f * jumpCount);
                }
            }
        }

        IEnumerator StackJump()
        {
            yield return new WaitForSeconds(0.1f);

            if (jumping == true && rigidbody.velocity.X * rigidbody.velocity.X > 1)
            {
                if (jumpCount < 3)
                {
                    jumpCount++;
                }
            }
            else
            {
                jumpCount = 0;
            }
        }

        private void PickUp()
        {
            if (Input.IsKeyDownThisFrame(binding.pickup))
            {
                if (player.CanPickUp(out Pickable pickable))
                {
                    if (NetManager.Instance.IsServer == false)
                        ClientConnectionComponent.Instance.RequestPickupWeapon(pickable);
                    else
                        player.PickUp(pickable);
                }
            }
        }

        private void Drop()
        {
            if (Input.IsKeyDownThisFrame(binding.weaponDrop))
            {
                if (NetManager.Instance.IsServer == false)
                {
                    ClientConnectionComponent.Instance.RequestWeaponDrop();
                    return;
                }

                equip.DropWeapon();
            }
        }

        private void Look()
        {
            if (Input.KeyDown(binding.lookRight))
            {
                player.lookingAt = PlayerManager.LookingAt.Right;
            }
            //else if (Input.KeyDown(binding.lookUp))
            //{
            //    player.lookingAt = PlayerManager.LookingAt.Up;
            //}
            else if (Input.KeyDown(binding.lookLeft))
            {
                player.lookingAt = PlayerManager.LookingAt.Left;
            }
            
        }
        private void Shoot()
        {
            if (Input.IsKeyDownThisFrame(binding.weaponFire) && equip.ActiveWeapon != null)
            {
                equip.ActiveWeapon.CoreMethod();
            }
        }

    }
}
