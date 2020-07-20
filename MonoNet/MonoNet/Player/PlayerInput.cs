using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.GameSystems.PickUps;

namespace MonoNet.Player
{
    public class PlayerInput : Component, Interfaces.IUpdateable
    {
        private PlayerManager player;
        private Rigidbody rigidbody;
        private PlayerKeys binding;
        private Transform2 transform;
        private Equip equip;

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
            Vector2 addVel = new Vector2();

            if (Input.KeyDown(binding.right))
            {
                addVel.X += player.XSpeed;
            }
            if (Input.KeyDown(binding.left))
            {
                addVel.X -= player.XSpeed;
            }
            addVel.X *= Time.Delta;

            if (rigidbody.grounded == true && Input.IsKeyDownThisFrame(binding.jump))
            {
                addVel.Y += player.JumpForce;
            }

            if (Input.IsKeyDownThisFrame(binding.pickup))
            {

            }

            if (Input.IsKeyDownThisFrame(binding.weaponFire))
            {
                equip.ActiveWeapon.CoreMethod();
            }
            if (Input.IsKeyDownThisFrame(binding.weaponDrop))
            {
                equip.DropWeapon();
            }

            rigidbody.velocity += addVel;
        }
    }
}
